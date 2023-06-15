using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using IdOps.Api;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage.Mongo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Squadron;

namespace IdOps.GraphQL.Tests
{
    public abstract class TestHelper
    {
        protected TestHelper(MongoResource mongoResource)
        {
            MongoResource = mongoResource;

            CreateNewService(true);
            DbContext = Services.GetRequiredService<IIdOpsDbContext>();
        }

        public ServiceProvider Services { get; private set; }

        public IIdOpsDbContext DbContext { get; }

        public MongoResource MongoResource { get; }

        public void NewServiceForUserWithNoPermission()
        {
            CreateNewService(false);
        }

        public void CreateNewService(bool permission)
        {
            var serviceCollection = new ServiceCollection();

            var configurationBuilder = new ConfigurationBuilder();

            ConfigureConfiguration(configurationBuilder, MongoResource);
            new Startup(configurationBuilder.Build(), HostEnvironment).ConfigureServices(
                serviceCollection);

            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(UserContextFactory(permission));

            RemoveSpecificEncryptionProvider(serviceCollection);
            

            Services = serviceCollection.BuildServiceProvider();
        }

        protected virtual void RemoveSpecificEncryptionProvider(
            IServiceCollection services)
        {
            //Removes Azure Keyvault in tests, should be overwritten if use is desired
            services.Remove(ServiceDescriptor
                .Singleton<ICryptographyClientProvider, AzureKeyvaultCryptographyClientProvider>());
            var cryptographyClientProvider = Mock.Of<ICryptographyClientProvider>();
            services.AddSingleton(p => cryptographyClientProvider);
        }
        

        public static IUserContextFactory UserContextFactory(bool permission)
        {
            Mock<IUserContext> userContext = new Mock<IUserContext>();
            userContext.Setup(f => f.HasPermission(It.IsAny<string>())).Returns(permission);
            userContext.Setup(f => f.GetTenantsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new string[] { "TestTenant" });

            Mock<IUserContextFactory> mock = new Mock<IUserContextFactory>();
            mock.Setup(t => t.Create()).Returns(userContext.Object);
            mock.Setup(t => t.Create(It.IsAny<ClaimsPrincipal>())).Returns(userContext.Object);

            return mock.Object;
        }

        public static IWebHostEnvironment HostEnvironment
        {
            get
            {
                Mock<IWebHostEnvironment> webHostEnv =
                    new Mock<IWebHostEnvironment>(MockBehavior.Strict);

                webHostEnv.Setup(m => m.EnvironmentName).Returns("Development"); // Development

                return webHostEnv.Object;
            }
        }

        public ValueTask<IRequestExecutor> CreateSchema() => Services.GetRequestExecutorAsync();

        public static void ConfigureConfiguration(IConfigurationBuilder builder,
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

        public async Task InsertTemplateIntoDB()
        {
            HashSet<Guid> apiScopes = new HashSet<Guid>();
            apiScopes.Add(Guid.Parse("4986e94402f04afeb0768288457b7bf1"));
            apiScopes.Add(Guid.Parse("d5684d9f86a747f187c161ed2bc3407c"));

            HashSet<Guid> identityScopes = new HashSet<Guid>();
            identityScopes.Add(Guid.Parse("2c654c8558a44236bdd5e9ad40beb953"));

            ClientTemplateSecret secret = new ClientTemplateSecret
            {
                EnvironmentId = Guid.Parse("34ca1d86dd6d4837bb9aeeeeb591fef9"),
                Type = "SharedSecret",
                Value = "asdfsadg"
            };
            ICollection<ClientTemplateSecret> secretList = new List<ClientTemplateSecret>();
            secretList.Add(secret);

            ClientTemplate template = new ClientTemplate
            {
                Id = Guid.Parse("443680dbca114aa09a5b6eceb1ba1671"),
                Name = "Template1",
                Tenant = "TestTenant",
                ApiScopes = apiScopes,
                IdentityScopes = identityScopes,
                Secrets = secretList
            };

            await DbContext.ClientTemplates.InsertOneAsync(template);
        }

        public async Task InsertTemplateOfWrongTenant()
        {
            HashSet<Guid> apiScopes = new HashSet<Guid>();
            apiScopes.Add(Guid.Parse("4986e94402f04afeb0768288457b7bf1"));
            apiScopes.Add(Guid.Parse("d5684d9f86a747f187c161ed2bc3407c"));

            HashSet<Guid> identityScopes = new HashSet<Guid>();
            identityScopes.Add(Guid.Parse("2c654c8558a44236bdd5e9ad40beb953"));

            ClientTemplateSecret secret = new ClientTemplateSecret
            {
                EnvironmentId = Guid.Parse("34ca1d86dd6d4837bb9aeeeeb591fef9"),
                Type = "SharedSecret",
                Value = "asdfsadg"
            };
            ICollection<ClientTemplateSecret> secretList = new List<ClientTemplateSecret>();
            secretList.Add(secret);

            ClientTemplate template = new ClientTemplate
            {
                Id = Guid.Parse("443680dbca114aa09a5b6eceb1ba1671"),
                Name = "Template1",
                Tenant = "Test",
                ApiScopes = apiScopes,
                IdentityScopes = identityScopes,
                Secrets = secretList
            };

            await DbContext.ClientTemplates.InsertOneAsync(template);
        }

        public async Task InsertTenantIntoDB()
        {
            Tenant tenant = new Tenant { Id = "TestTenant", Description = "testing" };

            await DbContext.Tenants.InsertOneAsync(tenant);
        }

        public async Task InsertWrongTenantIntoDB()
        {
            Tenant tenant = new Tenant { Id = "Test", Description = "wrong tenant" };

            await DbContext.Tenants.InsertOneAsync(tenant);
        }

        public async Task InsertApiScopeIntoDB()
        {
            ApiScope scope = new ApiScope
            {
                Id = Guid.Parse("aaa375c78dfa4730a7a22f4a805c419a"),
                Name = "scope",
                Tenant = "TestTenant",
                Enabled = true
            };

            await DbContext.ApiScopes.InsertOneAsync(scope);
        }
    }
}
