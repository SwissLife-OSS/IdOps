namespace IdOps
{
    public interface IResourceAuthoring
    {
        IApiResourceService ApiResources { get; }
        IClientService Clients { get; }
        IApiScopeService ApiScopes { get; }
        IIdentityResourceService IdentityResources { get; }
        IUserClaimRulesService UserClaimRules { get; }
        IApplicationService Applications { get; }
        IEnvironmentService Environments { get; }
        ITenantService Tenants { get; }
    }
}
