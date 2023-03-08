using IdOps.Model;

namespace IdOps.GraphQL;

public class GetClientSecretPayload
{
    public GetClientSecretPayload(Client client, string secret)
    {
        Client = client;
        Secret = secret;
    }

    public Client? Client { get; }

    public string? Secret { get; }
}
