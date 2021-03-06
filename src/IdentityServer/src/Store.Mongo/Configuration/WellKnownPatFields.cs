using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage.Mongo
{
    public static class WellKnownPatFields
    {
        public static readonly string ExpiresAt =
            $"{nameof(IdOpsPersonalAccessToken.Tokens)}.{nameof(IdOpsHashedToken.ExpiresAt)}";

        public static readonly string Token =
            $"{nameof(IdOpsPersonalAccessToken.Tokens)}.{nameof(IdOpsHashedToken.Token)}";

        public static readonly string IsUsed =
            $"{nameof(IdOpsPersonalAccessToken.Tokens)}.{nameof(IdOpsHashedToken.IsUsed)}";
    }
}
