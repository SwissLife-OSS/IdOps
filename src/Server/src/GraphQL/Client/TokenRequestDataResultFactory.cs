using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Abstractions;
using IdOps.Model;
using IdOps.Models;

namespace IdOps.GraphQL;

public class TokenRequestDataResultFactory : IResultFactory<TokenRequestData, TokenRequestInput>
{
    private readonly IEncryptionService _encryptionService;
    private readonly IClientService _clientService;
    private readonly IApiScopeService _scopeService;

    public TokenRequestDataResultFactory(
        IEncryptionService encryptionService,
        IClientService clientService,
        IApiScopeService apiScopeService)
    {
        _encryptionService = encryptionService;
        _clientService = clientService;
        _scopeService = apiScopeService;
    }


    public async Task<TokenRequestData> Create(TokenRequestInput input,
        CancellationToken cancellationToken)
    {
        Client? client = await _clientService.GetByIdAsync(input.ClientId, cancellationToken);
        if (client == null)
        {
            throw new ArgumentNullException($"Element with ID {input.ClientId} not found.");
        }

        var clientId = client.ClientId;

        string secretEncrypted =
            client.ClientSecrets.First(secret => secret.Id.Equals(input.SecretId)).EncryptedSecret;
        var secretDecrypted =
            await _encryptionService.DecryptAsync(secretEncrypted, cancellationToken);

        var grantTypes = client.AllowedGrantTypes.First();

        var scopeIds =
            client.AllowedScopes.ToList().ConvertAll(clientScope => clientScope.Id );
        var scopes =
            await _scopeService.GetByIdsAsync(scopeIds,cancellationToken);
        var scopeNames = scopes.Select(scope => scope.Name).ToList();

        var tokenRequestData =
            new TokenRequestData(
                input.Authority,
                clientId, secretDecrypted,
                grantTypes,
                scopeNames,
                input.Parameters)
                { RequestId = input.RequestId,
                    SaveTokens = input.SaveTokens };

        return tokenRequestData;
    }
}
