using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class IdentityResource : Resource, IResource
    {
        public Guid Id { get; set; }

        public ICollection<string> Tenants { get; set; } = new List<string>();

        public bool Required { get; set; } = false;

        public bool Emphasize { get; set; } = false;

        public bool ShowInDiscoveryDocument { get; set; } = true;

        public string Title => Name;

        public Guid IdentityServerGroupId { get; set; }

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Id == IdentityServerGroupId;
        }
    }
}
