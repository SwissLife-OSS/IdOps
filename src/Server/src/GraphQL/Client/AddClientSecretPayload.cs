using IdOps.Model;

namespace IdOps.GraphQL
{
    public class AddClientSecretPayload
    {
        public AddClientSecretPayload(Client client, string secret)
        {
            Client = client;
            Secret = secret;
        }

        public Client? Client { get; }

        public string? Secret { get; }
    }
}
