using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ITokenAnalyzer _tokenAnalyzer;

        public IdentityService(
            IHttpClientWrapper httpClientWrapper,
            ITokenAnalyzer tokenAnalyzer)
        {
            _httpClientWrapper = httpClientWrapper;
            _tokenAnalyzer = tokenAnalyzer;
        }

        public async Task<RequestTokenResult> RequestTokenAsync(
            TokenRequestData request,
            CancellationToken cancellationToken)
        {
            DiscoveryDocumentResponse disco = await _httpClientWrapper.GetDiscoveryDocumentAsync(
                request.Authority,
                cancellationToken);
            TokenResponse? response = null;

            if (request.GrantType == "client_credentials")
            {
                response = await _httpClientWrapper.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = request.ClientId,
                        ClientSecret = request.Secret,
                        GrantType = request.GrantType,
                        Scope = request.Scopes.Any() ? string.Join(" ", request.Scopes) : null
                    }, CancellationToken.None);
            }
            else
            {
                var pars = request.Parameters.ToDictionary(k => k.Name, v => v.Value);

                if (request.Scopes is { } s && s.Any())
                {
                    pars.Add("scope", string.Join(" ", request.Scopes));
                }

                response = await _httpClientWrapper.RequestTokenAsync(
                    new TokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = request.ClientId,
                        ClientSecret = request.Secret,
                        GrantType = request.GrantType,
                        Parameters = new Parameters(pars)
                    }, CancellationToken.None);
            }

            if (!response!.IsError)
            {
                TokenModel? accessToken = _tokenAnalyzer.Analyze(response.AccessToken);
                
                return new RequestTokenResult(true)
                {
                    AccessToken = accessToken
                };
            }
            else
            {
                return new RequestTokenResult(false)
                {
                    ErrorMessage = response.Error
                };
            }
        }
    }
}
