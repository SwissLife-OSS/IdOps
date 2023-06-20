using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps
{
    public class IdentityService : IIdentityService
    {
        private readonly ITokenClient _tokenClient;
        private readonly ITokenAnalyzer _tokenAnalyzer;

        public IdentityService(
            ITokenClient tokenClient,
            ITokenAnalyzer tokenAnalyzer)
        {
            _tokenClient = tokenClient;
            _tokenAnalyzer = tokenAnalyzer;
        }

        public async Task<RequestTokenResult> RequestTokenAsync(
            TokenRequestData request,
            CancellationToken cancellationToken)
        {
            DiscoveryDocumentResponse disco = await _tokenClient.GetDiscoveryDocumentAsync(
                request.Authority,
                cancellationToken);

            TokenResponse response = request.GrantType switch
            {
                "client_credentials" => await RequestClientCredentialTokenAsync(request, disco),
                _ => await RequestOtherGrantTypeTokenAsync(request, disco)
            };

            if (response.IsError)
            {
                return new RequestTokenResult(false) { ErrorMessage = response.Error };
            }
            
            TokenModel? accessToken = _tokenAnalyzer.Analyze(response.AccessToken);
            return new RequestTokenResult(true)
            {
                AccessToken = accessToken
            };
        }

        private async Task<TokenResponse> RequestClientCredentialTokenAsync(
            TokenRequestData request, 
            DiscoveryDocumentResponse disco)
        {
            return await _tokenClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.Secret,
                    GrantType = request.GrantType,
                    Scope = request.Scopes.Any() ? string.Join(" ", request.Scopes) : null
                }, CancellationToken.None);
        }

        private async Task<TokenResponse> RequestOtherGrantTypeTokenAsync(
            TokenRequestData request,
            DiscoveryDocumentResponse disco)
        {
            var pars = request.Parameters.ToDictionary(k => k.Name, v => v.Value);

            if (request.Scopes is { } s && s.Any())
            {
                pars.Add("scope", string.Join(" ", request.Scopes));
            }

            return await _tokenClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.Secret,
                    GrantType = request.GrantType,
                    Parameters = new Parameters(pars)
                }, CancellationToken.None);
        }
    }
}
