using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IdOps.IdentityServer.Samples
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
                });

        private static void SetupConfiguration(
            WebHostBuilderContext builderContext,
            IConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json");
            builder.AddJsonFile("appsettings.user.json", optional: true);
            builder.AddEnvironmentVariables();
        }
    }
}
