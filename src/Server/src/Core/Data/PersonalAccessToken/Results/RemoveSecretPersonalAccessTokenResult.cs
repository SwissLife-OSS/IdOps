using IdOps.Model;

namespace IdOps
{
    public class RemoveSecretPersonalAccessTokenResult
    {
        public RemoveSecretPersonalAccessTokenResult(
            PersonalAccessToken? token,
            HashedToken? hash)
        {
            Token = token;
            Hash = hash;
        }

        public PersonalAccessToken? Token { get; }

        public HashedToken? Hash { get; }
    }
}
