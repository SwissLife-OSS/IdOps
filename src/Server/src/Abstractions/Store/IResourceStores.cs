using IdOps.Server.Storage;

namespace IdOps
{
    public interface IResourceStores
    {
        IApiResourceStore ApiResources { get; }
        IApiScopeStore ApiScopes { get; }
        IClientStore Clients { get; }
        IPersonalAccessTokenStore PersonalAccessTokens { get; }
        IIdentityResourceStore IdentityResources { get; }
        IEnvironmentStore Environments { get; }
        ITenantStore Tenants { get; }
        IUserClaimRuleStore UserClaimRules { get; }
        IApplicationStore Applications { get; }
    }
}
