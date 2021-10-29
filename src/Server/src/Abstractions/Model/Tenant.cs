using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class Tenant
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string? Color { get; set; }

        public ICollection<TenantModule> Modules { get; set; } = new List<TenantModule>();

        public ICollection<TenantRoleMapping> RoleMappings { get; set; } = new List<TenantRoleMapping>();

        public ICollection<string> Emails { get; set; } = new List<string>();
    }

    public class TenantRoleMapping
    {
        public string Role { get; set; }

        public string ClaimValue { get; set; }

        public Guid? EnvironmentId { get; set; }
    }

    public class TenantModule
    {
        public string Name { get; set; }

        public IEnumerable<TenantSetting>? Settings { get; set; } = new List<TenantSetting>();
    }

    public class TenantSetting
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
