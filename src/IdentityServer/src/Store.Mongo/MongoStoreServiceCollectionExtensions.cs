using System;
using IdOps;
using IdOps.Exceptions;
using IdOps.IdentityServer;
using IdOps.IdentityServer.Store;
using IdOps.IdentityServer.Storage.Mongo;
using Microsoft.Extensions.Configuration;
using MongoDB.Extensions.Context;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoStoreServiceCollectionExtensions
    {
        public static IIdOpsIdentityServerBuilder UseMongoStores(
            this IIdOpsIdentityServerBuilder builder)
        {
            builder.Services.AddMongoStores(builder.Configuration);

            return builder;
        }

        public static IServiceCollection UseMongoStores(
            this IIdOpsIdentityServerBuilder builder,
            Action<IdOpsMongoOptions> setupAction)
        {
            var mongoOptions = new IdOpsMongoOptions();
            setupAction.Invoke(mongoOptions);

            return builder.Services.AddMongoStores(mongoOptions);
        }

        public static IServiceCollection AddMongoStores(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            IdOpsMongoOptions dbOptions = GetMongoDbOptions(configuration);

            return services.AddMongoStores(dbOptions);
        }

        public static IServiceCollection AddMongoStores(
            this IServiceCollection services,
            IdOpsMongoOptions dbOptions)
        {
            services.AddSingleton(dbOptions);
            services.AddSingleton<IIdentityStoreDbContext>(new IdentityStoreDbContext(dbOptions));
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IPersistedGrantRepository, PersistedGrantRepository>();
            services.AddSingleton<IApiScopeRepository, ApiScopeRepository>();
            services.AddSingleton<IApiResourceRepository, ApiResourceRepository>();
            services.AddSingleton<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddSingleton<IPersonalAccessTokenRepository, PersonalAccessTokenRepository>();
            services.AddSingleton<IUserClaimRuleRepository, UserClaimRuleRepository>();
            services.AddSingleton<
                IUserDataConnectorDataRepository,
                UserDataConnectorDataRepository>();

            return services;
        }

        private static IdOpsMongoOptions GetMongoDbOptions(IConfiguration configuration)
        {
            IdOpsMongoOptions options = configuration
                .GetSection(Wellknown.ConfigSections.Database)
                .Get<IdOpsMongoOptions>();

            if (options == null)
            {
                throw new IdOpsConfigurationException(
                    $@"Could not load configuration for Database, make sure that the
                      section: '{Wellknown.ConfigSections.Database}' is defined");
            }

            if (options.ConnectionString == null)
            {
                throw new IdOpsConfigurationException(
                    $@"Database ConnectionString can not be null, make sure
                      key: '{Wellknown.ConfigSections.Database}:ConnectionString' is defined");
            }

            if (options.DatabaseName == null)
            {
                throw new IdOpsConfigurationException(
                    $@"DatabaseName can not be null, make sure
                      key: '{Wellknown.ConfigSections.Database}:DatabaseName' is defined");
            }

            return options;
        }
    }
}
