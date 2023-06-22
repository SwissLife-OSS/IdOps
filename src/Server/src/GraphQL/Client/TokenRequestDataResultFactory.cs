using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Abstractions;
using IdOps.Model;
using IdOps.Models;

namespace IdOps.GraphQL;

public class TokenRequestDataResultFactory : IResultFactory<TokenRequestData, RequestTokenInput>
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


    public async Task<TokenRequestData> Create(
        RequestTokenInput input,
        CancellationToken cancellationToken)
    {
        Client? client = await _clientService.GetByIdAsync(input.ClientId, cancellationToken);
        if (client == null)
        {
            throw new ArgumentNullException($"Element with ID {input.ClientId} not found.");
        }

        var clientId = client.ClientId;

        var secretEncrypted = client.ClientSecrets.First(secret => secret.Id.Equals(input.SecretId))
            .EncryptedValue;
        if (secretEncrypted == null)
        {
            throw new KeyNotFoundException("No encrypted secret found");
        }

        var secretDecrypted =
            await _encryptionService.DecryptAsync(secretEncrypted, cancellationToken);

        if (client.AllowedGrantTypes == null)
        {
            throw new ArgumentNullException("No grant types found");
        }

        string grantType = client.AllowedGrantTypes.Contains(input.grantType)
            ? input.grantType
            : throw new ArgumentException("Grant not valid");

        var scopeIds = client.AllowedScopes.ToList().ConvertAll(clientScope => clientScope.Id);
        var scopes = await _scopeService.GetByIdsAsync(scopeIds, cancellationToken);
        var scopeNames = scopes.Select(scope => scope.Name).ToList();

        var tokenRequestData =
            new TokenRequestData(input.Authority, clientId, secretDecrypted, grantType, scopeNames)
            {
                SaveTokens = input.SaveTokens
            };

        return tokenRequestData;
    }
}
