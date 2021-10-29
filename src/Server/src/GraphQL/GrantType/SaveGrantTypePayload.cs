using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveGrantTypePayload: Payload
    {
        public SaveGrantTypePayload(GrantType grantType)
        {
            GrantType = grantType;
        }

        public GrantType GrantType { get; }
    }
}
