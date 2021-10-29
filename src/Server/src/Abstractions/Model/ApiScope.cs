using System;

namespace IdOps.Model
{
    public class ApiScope : Resource, ITenantResource
    {
        public Guid Id { get; set; }

        public string Tenant { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        public string Title => Name;

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }
    }
}
