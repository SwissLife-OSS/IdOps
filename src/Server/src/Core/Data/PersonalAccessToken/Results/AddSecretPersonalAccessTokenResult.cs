using IdOps.Model;

namespace IdOps
{
    public class AddSecretPersonalAccessTokenResult
    {
        public AddSecretPersonalAccessTokenResult(
            PersonalAccessToken? token,
            string? secret)
        {
            Secret = secret;
            Token = token;
        }

        public PersonalAccessToken? Token { get; }

        public string? Secret { get; }
    }
}
