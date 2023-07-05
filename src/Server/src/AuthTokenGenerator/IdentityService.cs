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
                "client_credentials" => await RequestClientCredentialTokenAsync(request, disco,cancellationToken),
                _ => await RequestOtherGrantTypeTokenAsync(request, disco, cancellationToken)
            };

            if (response.IsError)
            {
                RequestTokenResult result = new RequestTokenResult(false);

                result.ErrorMessage = response.Error switch
                {
                    "Unauthorized" => "Unauthorized",
                    "invalid_client" => "Invalid_client",
                    _ => "Unexpected_error"
                };

                return result;
            }
            
            TokenModel? accessToken = _tokenAnalyzer.Analyze(response.AccessToken);
            if (accessToken == null)
            {
                return new RequestTokenResult(false)
                {
                    ErrorMessage = "Access Token could not be analyzed"
                };
            }
            return new RequestTokenResult(true)
            {
                AccessToken = accessToken
            };
        }
        
        public async Task<RequestTokenResult> RequestTokenAsync(
            TokenRequest request,
            CancellationToken cancellationToken)
        {

            TokenResponse response = request.GrantType switch
            {
                "authorization_request" => await _tokenClient.RequestAuthorizationCodeTokenAsync(
                    (AuthorizationCodeTokenRequest) request ,
                    cancellationToken),
                _ => throw new Exception()
            };

            if (response.IsError)
            {
                RequestTokenResult result = new RequestTokenResult(false);

                result.ErrorMessage = response.Error switch
                {
                    "Unauthorized" => "Unauthorized",
                    "invalid_client" => "Invalid_client",
                    _ => "Unexpected_error"
                };

                return result;
            }
            
            TokenModel? accessToken = _tokenAnalyzer.Analyze(response.AccessToken);
            if (accessToken == null)
            {
                return new RequestTokenResult(false)
                {
                    ErrorMessage = "Access Token could not be analyzed"
                };
            }
            return new RequestTokenResult(true)
            {
                AccessToken = accessToken
            };
        }
        

        private async Task<TokenResponse> RequestClientCredentialTokenAsync(
            TokenRequestData request, 
            DiscoveryDocumentResponse disco, 
            CancellationToken cancellationToken)
        {
            return await _tokenClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.Secret,
                    GrantType = request.GrantType,
                    Scope = request.Scopes.Any() ? string.Join(" ", request.Scopes) : null
                }, cancellationToken);
        }

        private async Task<TokenResponse> RequestAuthorizationCodeTokenAsync(
            AuthorizationCodeTokenRequest request, 
            DiscoveryDocumentResponse disco,
            CancellationToken cancellationToken)
        {
            return await _tokenClient.RequestAuthorizationCodeTokenAsync(
                new AuthorizationCodeTokenRequest(), cancellationToken);
        }

        private async Task<TokenResponse> RequestOtherGrantTypeTokenAsync(
            TokenRequestData request,
            DiscoveryDocumentResponse disco,
            CancellationToken cancellationToken)
        {
            var pars = new Dictionary<string, string>();

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
                }, cancellationToken);
        }
    }
}
