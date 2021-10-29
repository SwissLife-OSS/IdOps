using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.Importer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider? sp = BuildServiceProvider();
            ResourceImporter? importer = sp.GetRequiredService<ResourceImporter>();

            await importer.ImportAsync(default);
            await importer.ImportClientsAsync();

            //await importer.CreateApplications(default);
        }

        static IServiceProvider BuildServiceProvider()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<ResourceImporter>()
                .AddEnvironmentVariables();

            IConfigurationRoot config = builder.Build();
            var services = new ServiceCollection();
            services.AddIdOpsServer(config)
                .AddMongoStore();

            services.AddSingleton<ResourceImporter>();
            return services.BuildServiceProvider();
        }
    }
}
