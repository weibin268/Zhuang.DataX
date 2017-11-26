using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Zhuang.Data.Configuration
{
    public class MyConfiguration
    {
        private static IConfiguration _configuration;
        private static object _objLock = new object();

        public static IConfiguration Get()
        {
            if (_configuration == null)
            {
                lock (_objLock)
                {
                    if (_configuration == null)
                    {
                        _configuration = Create();
                    }
                }
            }

            return _configuration;
        }

        public static IConfiguration Create()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            builder.AddJsonFile("appsettings.json", true, true);
            
            return builder.Build();
        }
    }
}
