using IdOps.Model;

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
