using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class IdentityServerGroup
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> Tenants { get; set; }

        public string? Color { get; set; }
    }
}
