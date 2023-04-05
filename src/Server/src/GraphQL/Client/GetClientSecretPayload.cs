using IdOps.Model;

namespace IdOps.GraphQL;

public class GetClientSecretPayload
{
    public GetClientSecretPayload(Client client, string secret, params IError[] errors)
    {
        Client = client;
        Secret = secret;
        Errors = errors;
    }

    public Duende.IdentityServer.Models.Client? Client { get; }

    public string? Secret { get; }

    public IError[] Errors { get; }
}
