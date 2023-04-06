using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Abstractions;
using IdOps.Encryption;
using IdOps.Model;

namespace IdOps.GraphQL;

public class TokenRequestDataFactory : IFactory<TokenRequestData, TokenRequestInput>
{
    private IEncryptionService _encryptionService;
    private IClientService _clientService;

    public TokenRequestDataFactory(IEncryptionService encryptionService,
        IClientService clientService)
    {
        _encryptionService = encryptionService;
        _clientService = clientService;
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

        //var scopes = client.AllowedScopes.Cast<string>();
        var scopes = new List<string> { "api.read" };


        var tokenRequestData =
            new TokenRequestData(
                input.Authority,
                clientId, secretDecrypted,
                grantTypes,
                scopes,
                input.Parameters)
                { RequestId = input.RequestId,
                    SaveTokens = input.SaveTokens };

        return tokenRequestData;
    }
}
