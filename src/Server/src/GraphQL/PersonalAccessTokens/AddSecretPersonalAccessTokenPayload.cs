using IdOps.Model;

namespace IdOps.GraphQL
{
    public class AddSecretPersonalAccessTokenPayload
    {
        public AddSecretPersonalAccessTokenPayload(
            PersonalAccessToken? token,
            string? secret)
        {
            Token = token;
            Secret = secret;
        }

        public PersonalAccessToken? Token { get; }

        public string? Secret { get; }

        public static AddSecretPersonalAccessTokenPayload From(
            AddSecretPersonalAccessTokenResult result)
        {
            return new(result.Token, result.Secret);
        }
    }
}
