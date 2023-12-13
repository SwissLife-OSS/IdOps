using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Execution;
using IdOps.Api;
using IdOps.Server.Storage.Mongo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Squadron;

namespace IdOps.GraphQL.Tests;

public abstract class TestHelper
{
    private readonly IIdOpsDbContext _dbContext;

    private readonly MongoResource _resource;

    protected TestHelper(MongoResource resource)
    {
        _resource = resource;
        Services = CreateNewService();
        _dbContext = Services.GetRequiredService<IIdOpsDbContext>();
    }

    protected ServiceProvider Services { get; }

    private ServiceProvider CreateNewService()
    {
        var serviceCollection = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder();

        ConfigureConfiguration(configurationBuilder, _resource);
        new Startup(configurationBuilder.Build(), HostEnvironment)
            .ConfigureServices(serviceCollection);

        serviceCollection.AddLogging();
        serviceCollection.AddSingleton<IRequestContextEnricher, HttpContextEnricher>();

        RemoveSpecificEncryptionProvider(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }

    protected virtual void RemoveSpecificEncryptionProvider(IServiceCollection services)
    {
        //Removes Azure Keyvault in tests, should be overwritten if use is desired
        var descriptor = ServiceDescriptor
            .Singleton<ICryptographyClientProvider, AzureKeyVaultCryptographyClientProvider>();
        services.Remove(descriptor);
        var cryptographyClientProvider = Mock.Of<ICryptographyClientProvider>();
        services.AddSingleton(_ => cryptographyClientProvider);
    }

    private static IWebHostEnvironment HostEnvironment
    {
        get
        {
            var webHostEnv =
                new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            webHostEnv.Setup(m => m.EnvironmentName).Returns("Development"); // Development

            return webHostEnv.Object;
        }
    }

    protected ValueTask<IRequestExecutor> CreateSchema() => Services.GetRequestExecutorAsync();

    private static void ConfigureConfiguration(
        IConfigurationBuilder builder,
        MongoResource mongoResource)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Tracing:Enabled"] = "false",
            ["Tracing:Forwarding:0:Regex"] =
                "Microsoft.AspNetCore.Hosting.Internal.WebHost",
            ["Tracing:Forwarding:0:Level"] = "Warning",
            ["IdOps:MutedClients:0"] = "Contoso.HealthCheck",
            ["IdOps:Messaging:Transport"] = "Memory",
            ["IdOps:Storage:Database:ConnectionString"] = mongoResource.ConnectionString,
            ["IdOps:Storage:Database:DatabaseName"] =
                mongoResource.CreateDatabase().DatabaseNamespace.DatabaseName,
            ["IdOps:Security:Authority"] = "",
            ["IdOps:Security:ClientId"] = null,
            ["IdOps:Security:Secret"] = null,
            ["IdOps:Azure:SecretEncryption:AzureKeyVault:KeyVaultUri"] = "https://xxx/",
            ["IdOps:Azure:SecretEncryption:AzureKeyVault:EncryptionKeyName"] = "xxx",
        });
    }
}
