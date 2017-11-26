﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Zhuang.Data.Common;
using Zhuang.Data.EnvironmentVariable;
using Zhuang.Data.Configuration;
using System.Linq;

namespace Zhuang.Data.SqlCommands.Store
{
    public class ConfigFilesProvider : ISqlCommandStoreProvider
    {
        internal const string CONFIG_FILE_EXTENSION = ".config";

        private FileSystemWatcher _watcher;

        private string _basePath;

        public string BasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_basePath))
                {
                    string defaultBasePath = @".\App_Config\SqlCommands";
                    string configBasePath = MyConfiguration.Get().GetZhuangData(AppSettingsKey.SqlCommandsBasePath);
                    string tempBasePath = configBasePath == null ? defaultBasePath : configBasePath;

                    _basePath = tempBasePath;
                }
                return _basePath;
            }
        }

        public ConfigFilesProvider()
        {
            if (!Directory.Exists(Path.GetFullPath(BasePath)))
            {
                Directory.CreateDirectory(Path.GetFullPath(BasePath));
            }

            _watcher = new FileSystemWatcher(Path.GetFullPath(BasePath), "*" + CONFIG_FILE_EXTENSION);
            _watcher.IncludeSubdirectories = true;
            // Add event handlers.
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        public void EnableFileSystemWatcher()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void DisableFileSystemWatcher()
        {
            _watcher.EnableRaisingEvents = false;
        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            HandleChanged();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private void HandleChanged()
        {
            var sqlCmds = GetSqlCommands();
            foreach (var sqlcmd in sqlCmds)
            {
                SqlCommandRepository.Instance.AddOrReplaceSqlCommand(sqlcmd);
            }
        }

        public IEnumerable<SqlCommand> GetSqlCommands()
        {
            List<SqlCommand> result = new List<SqlCommand>();

            DirectoryInfo dirRoot = new DirectoryInfo(Path.GetFullPath(BasePath));
            if (!dirRoot.Exists)
                dirRoot.Create();

            RecursiveGetSqlCommands(dirRoot, result);

            return result;
        }

        private void RecursiveGetSqlCommands(DirectoryInfo dir, List<SqlCommand> sqlcmds)
        {
            var files = dir.GetFiles();

            foreach (var file in files)
            {
                sqlcmds.AddRange(GetSqlCommandsFromFile(file));
            }

            var dirs = dir.GetDirectories();
            if (dirs.Length != 0)
            {
                foreach (var tempDir in dirs)
                {
                    RecursiveGetSqlCommands(tempDir, sqlcmds);
                }
            }
            else
            {
                return;
            }

        }

        private IEnumerable<SqlCommand> GetSqlCommandsFromFile(FileInfo file)
        {
            IList<SqlCommand> result = new List<SqlCommand>();

            //if (file.Extension.ToLower() != CONFIG_FILE_EXTENSION)
            if (!IsValidConfigFileName(file.Name))
                return result;

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                using (FileStream fs = new FileStream(file.FullName,FileMode.Open,FileAccess.Read))
                { 
                    xmlDoc.Load(fs);
                }
            }
            catch (Exception)
            {
                return result;
            }

            result = (List<SqlCommand>)GetSqlCommandsFromXmlDoc(xmlDoc);

            return result;
        }

        public static IEnumerable<SqlCommand> GetSqlCommandsFromXmlDoc(XmlDocument xmlDoc)
        {
            IList<SqlCommand> result = new List<SqlCommand>();

            var commandsNodeList = xmlDoc.GetElementsByTagName("commands");
            var commandsNode = commandsNodeList.Count > 0 ? commandsNodeList[0] : null;

            if (commandsNode == null)
                return result;

            var commandNodes = commandsNode.ChildNodes;

            foreach (XmlNode commandNode in commandNodes)
            {
                if (commandNode.Name != "command")
                    continue;

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Key = commandNode.Attributes["key"] == null ? "" : commandNode.Attributes["key"].Value;
                sqlCmd.Text = commandNode.InnerText;

                result.Add(sqlCmd);
            }

            return result;
        }

        public static bool IsValidConfigFileName(string fileName)
        {
            bool result = true;

            if (!fileName.EndsWith(CONFIG_FILE_EXTENSION))
            {
                result = false;
            }

            #region 如果配置文件的名字是以XXX.DbProviderName.config(如：XXX.sqlserver.config)形式出现的，则进行筛选处理，筛选出只能当前使用的Provider相同的配置文件
            var fileNamePieces = fileName.Split('.');
            if (fileNamePieces.Length > 2)
            {
                string configDbProviderName = fileNamePieces[fileNamePieces.Length - 2];

                var lsDbProviderNames = new List<string>();
                foreach (var field in typeof(DbProviderName).GetTypeInfo().GetFields(0))
                {
                    lsDbProviderNames.Add(field.Name);
                }

                if (lsDbProviderNames.Exists(c => c.ToLower() == configDbProviderName.ToLower()))
                {
                    string strDbProviderName = EnvValService.GetDefaultDbAccessorDbProviderName();
                    if (configDbProviderName.ToLower() != strDbProviderName.ToString().ToLower())
                    {
                        result = false;
                    }
                }
            }
            #endregion

            return result;
        }
    }
}
