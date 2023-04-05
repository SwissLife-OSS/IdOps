using Duende.IdentityServer.Models;

namespace IdOps.GraphQL
{
    public class SaveClientPayload
    {
        public SaveClientPayload(Client client)
        {
            Client = client;
        }

        public Client? Client { get; }
    }
}
