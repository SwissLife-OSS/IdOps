namespace IdOps.IdentityServer
{
    public record PersonalAccessTokenMatch(
        IdOpsPersonalAccessToken Definition,
        IdOpsHashedToken Token);
}
