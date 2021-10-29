using IdOps.Model;

namespace IdOps.GraphQL
{
    public class RemoveSecretPersonalAccessTokenPayload
    {
        public RemoveSecretPersonalAccessTokenPayload(
            PersonalAccessToken? token,
            HashedToken? hash)
        {
            Token = token;
            Hash = hash;
        }

        public PersonalAccessToken? Token { get; }

        public HashedToken? Hash { get; }

        public static RemoveSecretPersonalAccessTokenPayload From(
            RemoveSecretPersonalAccessTokenResult result)
        {
            return new(result.Token, result.Hash);
        }
    }
}
