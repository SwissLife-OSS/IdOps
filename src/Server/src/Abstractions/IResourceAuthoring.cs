namespace IdOps
{
    public interface IResourceAuthoring
    {
        IApiResourceService ApiResources { get; }
        IClientService Clients { get; }
        IApplicationService Applications { get; }
        IEnvironmentService Environments { get; }
        ITenantService Tenants { get; }
    }
}
