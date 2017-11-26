using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Zhuang.Data.Common;
using Zhuang.Data.Configuration;
using Zhuang.Data.DbProviders.SqlServer;
using Zhuang.Data.EnvironmentVariable;
using Zhuang.Data.Handlers;
using Microsoft.Extensions.Configuration;

namespace Zhuang.Data
{
    public static class DbAccessorFactory
    {
        public static string DefaultDbName
        {
            get {

                string defaultDbName = MyConfiguration.Get().GetZhuangData(AppSettingsKey.DefaultDbName);
                return defaultDbName == null ? "DefaultDb" : defaultDbName;
            }
        }

        private static DbAccessor _dba;

        private static object _objLock = new object();

        public static DbAccessor GetDbAccessor()
        {
            if (_dba == null)
            {
                lock (_objLock)
                {
                    if (_dba == null)
                    {
                        _dba = CreateDbAccessor();
                        _dba.IsSingleton = true;
                    }
                }
            }
            return _dba;
        }

        public static DbAccessor CreateDbAccessor()
        {
            var result = CreateDbAccessor(DefaultDbName);

            EnvValService.SetDefaultDbAccessorDbProviderName(result);

            return result;
        }

        public static DbAccessor CreateDbAccessor(string name)
        {
            var connectionString = MyConfiguration.Get().GetConnectionString(name);
            var providerName = MyConfiguration.Get().GetConnectionStringProviderName(name);

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception(string.Format("请检查配置文件的数据库连接配置，找不到名称为{0}的ConnectionString！", name));

            
            return CreateDbAccessor(connectionString, providerName);

        }

        public static DbAccessor CreateDbAccessor(string connectionString, string providerName)
        {
            DbAccessor dba = NewDbAccessor(connectionString, providerName);

            if (dba != null)
            {
                IEnumerable<IDbExecuteHandler> dbExecuteHandlers = DbExecuteHandlerFactory.GetDbExecuteHandlers();

                foreach (IDbExecuteHandler handler in dbExecuteHandlers)
                {
                    dba.PreCommandExecute += handler.HandleExecute;
                }
            }

            return dba;
        }

        public static DbAccessor NewDbAccessor(string connectionString, string providerName)
        {
            DbAccessor dba = null;

            if (string.IsNullOrEmpty(providerName)
                || providerName == "System.Data.SqlClient"
                || providerName.ToLower() == DbProviderName.SqlServer.ToString().ToLower())
            {
                dba = new SqlServerAccessor(connectionString);
            }
            else
            {
                Type tProviderName = Type.GetType(providerName);
                if (tProviderName == null)
                {
                    throw new Exception(string.Format("ConnectionString（{0}）的ProviderName（{1}）找不到该类型！", connectionString, providerName));
                }
                //else if (!(tProviderName.IsSubclassOf(typeof(DbAccessor))))
                //{
                //    throw new Exception(string.Format("ConnectionString（{0}）的ProviderName（{1}）该类型不是DbAccessor的实现类！", connectionString, providerName));
                //}
                object oProviderName = Activator.CreateInstance(tProviderName, connectionString);
                dba = oProviderName as DbAccessor;

            }

            return dba;
        }
    }
}
