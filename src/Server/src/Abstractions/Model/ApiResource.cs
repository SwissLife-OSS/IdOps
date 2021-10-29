using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class ApiResource : Resource, ITenantResource
    {
        public Guid Id { get; set; }

        public string Tenant { get; set; }

        public bool RequireResourceIndicator { get; set; }

        public ICollection<Secret> ApiSecrets { get; set; } = new HashSet<Secret>();

        public ICollection<Guid> Scopes { get; set; } = new HashSet<Guid>();

        public ICollection<string> AllowedAccessTokenSigningAlgorithms { get; set; } = new HashSet<string>();

        public string Title => Name;

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }
    }
}
