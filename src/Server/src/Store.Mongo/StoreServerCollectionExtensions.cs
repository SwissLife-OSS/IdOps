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
            this IIdOpsServerBuilder builder)
        {
            MongoOptions options = builder.Configuration.GetSection("Storage:Database")
                .Get<MongoOptions>();

            builder.Services.AddSingleton<IIdOpsDbContext>(new IdOpsDbContext(options));
            builder.Services.AddStores();

            return builder;
        }

        private static IServiceCollection AddStores(this IServiceCollection services)
        {
            services.AddSingleton<IResourceStore<ApiResource>, IApiResourceStore, ApiResourceStore>();
            services.AddSingleton<IResourceStore<ApiScope>, IApiScopeStore, ApiScopeStore>();
            services.AddSingleton<IResourceStore<IdentityResource>, IIdentityResourceStore, IdentityResourceStore>();
            services.AddSingleton<IResourceStore<Client>, IClientStore, ClientStore>();
            services.AddSingleton<IResourceStore<Application>, IApplicationStore, ApplicationStore>();
            services.AddSingleton<IResourceStore<Application>, IApplicationStore, ApplicationStore>();
            services.AddSingleton<ITenantStore, TenantStore>();
            services.AddSingleton<IEnvironmentStore, EnvironmentStore>();
            services.AddSingleton<IGrantTypeStore, GrantTypeStore>();
            services.AddSingleton<IResouceAuditStore, ResouceAuditStore>();
            services.AddSingleton<IResourcePublishStateStore, ResourcePublishStateStore>();
            services.AddSingleton<IResourcePublishLogStore, ResourcePublishLogStore>();
            services.AddSingleton<IResourceApprovalStateStore, ResourceApprovalStateStore>();
            services.AddSingleton<IResourceApprovalLogStore, ResourceApprovalLogStore>();
            services.AddSingleton<IIdentityServerEventStore, IdentityServerEventStore>();
            services.AddSingleton<IResourceStore<Model.IdentityServer>, IIdentityServerStore, IdentityServerStore>();
            services.AddSingleton<IClientTemplateStore, ClientTemplateStore>();
            services.AddSingleton<IResourceStore<UserClaimRule>, IUserClaimRuleStore, UserClaimRulesStore>();
            services.AddSingleton<IResourceStore<PersonalAccessToken>, IPersonalAccessTokenStore, PersonalAccessTokenStore>();


            return services;
        }

        private static IServiceCollection AddSingleton<TService1, TService2, TImplementation>(
            this IServiceCollection services)
            where TService1 : class
            where TService2 : class, TService1
            where TImplementation : class, TService1, TService2
        {
            return services
                .AddSingleton<TService1, TImplementation>()
                .AddSingleton(sp => (TService2)sp.GetRequiredService<TService1>());
        }
    }
}
