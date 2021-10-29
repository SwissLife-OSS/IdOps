using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class Application : ITenantResource
    {
        public Guid Id { get; set; }

        public string Tenant { get; set; }

        public string Name { get; set; }

        public string Title => Name;

        public ICollection<Guid> ApiScopes { get; set; } = new HashSet<Guid>();

        public ICollection<Guid> IdentityScopes { get; set; } = new HashSet<Guid>();

        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        public ICollection<string> PostLogoutRedirectUris { get; set; } = new HashSet<string>();

        public ICollection<Guid> ClientIds { get; set; } = new HashSet<Guid>();

        public ResourceVersion Version { get; set; }

        public ICollection<string> AllowedGrantTypes { get; set; } = new HashSet<string>();

        public Guid TemplateId { get; set; }

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }
    }
}
