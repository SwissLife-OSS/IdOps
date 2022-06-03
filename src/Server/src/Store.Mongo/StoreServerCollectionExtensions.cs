using System;
using IdOps.Model;
using IdOps.Server.Storage;
using IdOps.Server.Storage.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace IdOps
{
    public static class StoreServerCollectionExtensions
    {
        public static IIdOpsServerBuilder AddMongoStore(
            this IIdOpsServerBuilder builder,
            Action<IMongoDatabaseBuilder>? configureMongoDatabaseBuilder = default)
        {
            MongoOptions options = builder.Configuration.GetSection("Storage:Database")
                .Get<MongoOptions>();

            builder.Services.AddSingleton<IIdOpsDbContext>(_ =>
            {
                var context = new IdOpsDbContext(options, configureMongoDatabaseBuilder);
                context.Initialize();

                return context;
            });

            builder.Services.AddStores();

            return builder;
        }

        private static IServiceCollection AddStores(this IServiceCollection services)
        {
            services.AddResourceStore<IResourceStore<ApiResource>, IApiResourceStore, ApiResourceStore>();
            services.AddResourceStore<IResourceStore<ApiScope>, IApiScopeStore, ApiScopeStore>();
            services.AddResourceStore<IResourceStore<IdentityResource>, IIdentityResourceStore, IdentityResourceStore>();
            services.AddResourceStore<IResourceStore<Client>, IClientStore, ClientStore>();
            services.AddResourceStore<IResourceStore<Application>, IApplicationStore, ApplicationStore>();
            services.AddSingleton<ITenantStore, TenantStore>();
            services.AddSingleton<IEnvironmentStore, EnvironmentStore>();
            services.AddSingleton<IGrantTypeStore, GrantTypeStore>();
            services.AddSingleton<IResouceAuditStore, ResouceAuditStore>();
            services.AddSingleton<IResourcePublishStateStore, ResourcePublishStateStore>();
            services.AddSingleton<IResourcePublishLogStore, ResourcePublishLogStore>();
            services.AddSingleton<IResourceApprovalStateStore, ResourceApprovalStateStore>();
            services.AddSingleton<IResourceApprovalLogStore, ResourceApprovalLogStore>();
            services.AddSingleton<IIdentityServerEventStore, IdentityServerEventStore>();
            services.AddResourceStore<IResourceStore<Model.IdentityServer>, IIdentityServerStore, IdentityServerStore>();
            services.AddSingleton<IIdentityServerGroupStore, IdentityServerGroupStore>();
            services.AddSingleton<IClientTemplateStore, ClientTemplateStore>();
            services.AddResourceStore<IResourceStore<UserClaimRule>, IUserClaimRuleStore, UserClaimRulesStore>();
            services.AddResourceStore<IResourceStore<PersonalAccessToken>, IPersonalAccessTokenStore, PersonalAccessTokenStore>();

            return services;
        }

        private static IServiceCollection AddResourceStore<TService1, TService2, TImplementation>(
            this IServiceCollection services)
            where TService1 : class, IResourceStore
            where TService2 : class, TService1
            where TImplementation : class, TService1, TService2
        {
            return services
                .AddSingleton<TService2, TImplementation>()
                .AddSingleton<TService1>(sp => sp.GetRequiredService<TService2>())
                .AddSingleton<IResourceStore>(sp => sp.GetRequiredService<TService1>());
        }
    }
}
