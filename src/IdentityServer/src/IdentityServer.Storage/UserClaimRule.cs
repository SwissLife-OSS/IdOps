using System;
using System.Collections.Generic;

namespace IdOps.IdentityServer.Model
{
    public class UserClaimRule : IdOpsResource
    {
        public string Name { get; set; } = default!;

        public string Tenant { get; set; } = default!;

        public IEnumerable<string> ClientIds { get; set; } = Array.Empty<string>();

        public IEnumerable<ClaimRuleMatch> Rules { get; set; } = Array.Empty<ClaimRuleMatch>();

        public IEnumerable<UserClaim> Claims { get; set; } = Array.Empty<UserClaim>();

        public PublishSource Source { get; set; }
    }
}
