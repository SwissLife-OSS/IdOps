using System;
using System.Collections.Generic;

namespace IdOps
{
    public record SaveIdentityServerGroupRequest(
        string Name,
        ICollection<string> Tenants,
        string Color)
    {
        public Guid? Id { get; init; }
    }
}
