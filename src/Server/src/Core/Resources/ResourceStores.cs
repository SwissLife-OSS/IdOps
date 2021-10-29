using System;
using IdOps.Store;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public class ResourceStores : IResourceStores
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceStores(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IClientStore Clients
            => _serviceProvider.GetRequiredService<IClientStore>();

        public IPersonalAccessTokenStore PersonalAccessTokens
            => _serviceProvider.GetRequiredService<IPersonalAccessTokenStore>();

        public IApiResourceStore ApiResources
            => _serviceProvider.GetRequiredService<IApiResourceStore>();

        public IIdentityResourceStore IdentityResources
            => _serviceProvider.GetRequiredService<IIdentityResourceStore>();

        public IApiScopeStore ApiScopes
            => _serviceProvider.GetRequiredService<IApiScopeStore>();

        public IEnvironmentStore Environments
            => _serviceProvider.GetRequiredService<IEnvironmentStore>();

        public ITenantStore Tenants
            => _serviceProvider.GetRequiredService<ITenantStore>();

        public IUserClaimRuleStore UserClaimRules
            => _serviceProvider.GetRequiredService<IUserClaimRuleStore>();

        public IApplicationStore Applications
            => _serviceProvider.GetRequiredService<IApplicationStore>();
    }
}
