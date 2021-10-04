using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Imagegram.Helpers
{
    public static class ConfigHelper
    {
        private static IConfiguration configuration;
        static ConfigHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
        }

        public static string GetValue(string name)
        {
            string appSettings = configuration[name];
            return appSettings;
        }
        public static string GetConnectionString(string name)
        {
            string appSettings = configuration.GetConnectionString(name);
            return appSettings;
        }
    }
}
