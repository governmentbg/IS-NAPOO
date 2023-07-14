using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAuth.Utils
{
    public class LogHelper
    {
        public static class LogConfiguration
        {
            public static string LogConfigurationPath { get; }

            static LogConfiguration()
            {
                IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

                LogConfigurationPath = configuration.GetSection("AppSettings")["LogConfigurationPath"] ?? "log4net.config";
            }
        }

    }
}
