using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SavePersonalAccessTokenPayload
    {
        public SavePersonalAccessTokenPayload(PersonalAccessToken token)
        {
            Token = token;
        }

        public PersonalAccessToken? Token { get; }
    }
}
