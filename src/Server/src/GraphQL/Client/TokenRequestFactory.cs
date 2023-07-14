using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Model;
using IdOps.Models;
using IdOps.Security;

namespace IdOps.GraphQL;

public class TokenRequestFactory : IResultFactory<TokenRequest, RequestTokenInput>
{
    private readonly IEncryptionService _encryptionService;
    private readonly IClientService _clientService;
    private readonly IApiScopeService _scopeService;
    private readonly IHttpClientFactory _httpClientFactory;

    public TokenRequestFactory(
        IEncryptionService encryptionService,
        IClientService clientService, 
        IApiScopeService apiScopeService,
        IHttpClientFactory httpClientFactory)
    {
        _encryptionService = encryptionService;
        _clientService = clientService;
        _scopeService = apiScopeService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TokenRequest> CreateRequestAsync(
        RequestTokenInput input,
        CancellationToken cancellationToken)
    {
        if (input.grantType == null)
        {
            throw new ArgumentNullException("No grants found");
        }

        return input.grantType switch
        {
            "client_credentials" => 
                await BuildClientCredentialsTokenRequestAsync(input, cancellationToken),
            "authorization_code" => 
                await BuildAuthorizationCodeTokenRequestAsync(input, cancellationToken),
            _ => throw new Exception("grant not valid")
        };
    }

    private async Task<ClientCredentialsTokenRequest> BuildClientCredentialsTokenRequestAsync(
        RequestTokenInput input, 
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco = await httpClient.GetDiscoveryDocumentAsync(input.Authority, cancellationToken);

        Client? client = await _clientService.GetByIdAsync(input.ClientId, cancellationToken);
        if (client == null)
        {
            throw new ArgumentNullException($"Element with ID {input.ClientId} not found.");
        }

        var clientId = client.ClientId;

        if (!client.AllowedGrantTypes.Contains(input.grantType))
        {
            throw new ArgumentException("Grant not valid");
        }

        var secretEncrypted = client.ClientSecrets.First(secret => secret.Id.Equals(input.SecretId)).EncryptedValue;
        if (secretEncrypted == null)
        {
            throw new KeyNotFoundException("No encrypted secret found");
        }

        var secretDecrypted = await _encryptionService.DecryptAsync(secretEncrypted, cancellationToken);

        if (client.AllowedGrantTypes == null)
        {
            throw new ArgumentNullException("Client has no registered grant types");
        }

        var scopeIds = client.AllowedScopes.Select(clientScope => clientScope.Id).ToList();
        var scopes = await _scopeService.GetByIdsAsync(scopeIds, cancellationToken);
        var scopeNames = string.Join(", ", scopes.Select(scope => scope.Name));

        return new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = clientId,
            ClientSecret = secretDecrypted,
            Scope = scopeNames,
            GrantType = "client_credentials"
        };
    }

    private async Task<AuthorizationCodeTokenRequest> BuildAuthorizationCodeTokenRequestAsync(
        RequestTokenInput input, 
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco = await httpClient.GetDiscoveryDocumentAsync(input.Authority, cancellationToken);

        Client? client = await _clientService.GetByIdAsync(input.ClientId, cancellationToken);
        if (client == null)
        {
            throw new ArgumentNullException($"Element with ID {input.ClientId} not found.");
        }

        var clientId = client.ClientId;

        if (!client.AllowedGrantTypes.Contains(input.grantType))
        {
            throw new ArgumentException("Grant not valid");
        }

        if (input.code == null)
        {
            throw new ArgumentNullException("No code found");
        }

        var secretEncrypted = client.ClientSecrets.First(secret => secret.Id.Equals(input.SecretId)).EncryptedValue;
        if (secretEncrypted == null)
        {
            throw new KeyNotFoundException("No encrypted secret found");
        }

        var secretDecrypted = await _encryptionService.DecryptAsync(secretEncrypted, cancellationToken);

        if (client.AllowedGrantTypes == null)
        {
            throw new ArgumentNullException("Client has no registered grant types");
        }

        return new AuthorizationCodeTokenRequest()
        {
            Address = disco.TokenEndpoint,
            ClientId = clientId,
            ClientSecret = secretDecrypted,
            Code = input.code,
            GrantType = "authorization_code"
            
        };
    }

}
