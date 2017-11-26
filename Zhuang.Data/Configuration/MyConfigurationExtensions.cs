using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Zhuang.Data.Utility;

namespace Zhuang.Data.Configuration
{
    public static class MyConfigurationExtensions
    {
        public static string GetZhuangData(this IConfiguration configuration, string name)
        {

            if (configuration == null)
            {
                return null;
            }

            IConfigurationSection section = configuration.GetSection("Zhuang:Data");
            if (section == null)
            {
                return null;
            }

            return section[name];
        }

        public static string GetConnectionStringProviderName(this IConfiguration configuration, string connectionStringName)
        {

            if (configuration == null)
            {
                return null;
            }

            IConfigurationSection section = configuration.GetSection("Zhuang:Data:ConnectionStringProviderName");
            if (section == null)
            {
                return null;
            }

            return section[connectionStringName];
        }

    }
}
