using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace IdOps.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(SetupConfiguration);
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureLogging(SetupLogging);

        private static void SetupLogging(
            HostBuilderContext hostBuilder,
            ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole()
                .SetMinimumLevel(LogLevel.Debug);
        }

        private static void SetupConfiguration(
            WebHostBuilderContext builderContext,
            IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());

            builder.AddJsonFile("appsettings.json");
            builder.AddJsonFile("appsettings.user.json", true);
            builder.AddUserSecrets<Startup>();
            builder.AddEnvironmentVariables();
        }
    }
}
