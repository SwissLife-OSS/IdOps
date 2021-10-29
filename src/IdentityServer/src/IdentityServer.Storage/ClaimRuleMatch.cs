namespace IdOps.IdentityServer.Model
{
    public class ClaimRuleMatch
    {
        public string ClaimType { get; set; } = default!;

        public string Value { get; set; } = default!;

        public ClaimRuleMatchMode MatchMode { get; set; } = ClaimRuleMatchMode.Equals;
    }
}
