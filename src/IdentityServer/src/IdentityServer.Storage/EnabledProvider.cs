namespace IdOps.IdentityServer.Model
{
    public class EnabledProvider
    {
        public string Name { get; set; }

        public bool RequestMfa { get; set; }

        public string? UserIdClaimType { get; set; }
    }
}
