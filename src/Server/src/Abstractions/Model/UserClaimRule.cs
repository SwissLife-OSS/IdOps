using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class UserClaimRule : ITenantResource
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Tenant { get; set; } = default!;

        public Guid? ApplicationId { get; set; }

        public ResourceVersion Version { get; set; } = default!;

        public string Title => Name;

        public IEnumerable<ClaimRuleMatch> Rules { get; set; } = Array.Empty<ClaimRuleMatch>();

        public IEnumerable<UserClaim> Claims { get; set; } = Array.Empty<UserClaim>();

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }
    }

    public class UserClaim
    {
        public string Type { get; set; } = default!;

        public string Value { get; set; } = default!;
    }

    public class ClaimRuleMatch
    {
        public Guid? EnvironmentId { get; set; }

        public string ClaimType { get; set; } = default!;

        public string Value { get; set; } = default!;

        public ClaimRuleMatchMode MatchMode { get; set; } = ClaimRuleMatchMode.Equals;

        public bool IsGlobal() => !EnvironmentId.HasValue;
    }

    public enum ClaimRuleMatchMode
    {
        Equals,
        Contains,
        StartWith,
        EndsWith,
        OneOf,
        Regex,
    }
}
