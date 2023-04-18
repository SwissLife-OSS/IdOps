using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps
{
    public class IdentityService : IIdentityService
    {
        //TODO: Extract clientFactory (& tokenAnalyzer) into OidcController for easier mock
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ITokenAnalyzer _tokenAnalyzer;
        private readonly IAuthTokenStore _authTokenStore;

        public IdentityService(
            IHttpClientFactory httpClientFactory,
            IHttpClientWrapper httpClientWrapper,
            ITokenAnalyzer tokenAnalyzer,
            IAuthTokenStore authTokenStore)
        {
            _httpClientFactory = httpClientFactory;
            _httpClientWrapper = httpClientWrapper;
            _tokenAnalyzer = tokenAnalyzer;
            _authTokenStore = authTokenStore;
        }

        public async Task<RequestTokenResult> RequestTokenAsync(
            TokenRequestData request,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse disco = await GetDiscoveryDocumentAsync(
                request.Authority,
                cancellationToken);
            TokenResponse? response = null;

            if (request.GrantType == "client_credentials")
            {
                response = await httpClient.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = request.ClientId,
                        ClientSecret = request.Secret,
                        GrantType = request.GrantType,
                        Scope = request.Scopes.Any() ? string.Join(" ", request.Scopes) : null
                    });
            }
            else
            {
                var pars = request.Parameters.ToDictionary(k => k.Name, v => v.Value);

                if (request.Scopes is { } s && s.Any())
                {
                    pars.Add("scope", string.Join(" ", request.Scopes));
                }

                response = await httpClient.RequestTokenAsync(
                    new TokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = request.ClientId,
                        ClientSecret = request.Secret,
                        GrantType = request.GrantType,
                        Parameters = new Parameters(pars)
                    });
            }

            if (!response!.IsError)
            {
                TokenModel? accessToken = _tokenAnalyzer.Analyze(response.AccessToken);

                if ( request.SaveTokens && request.RequestId.HasValue)
                {
                    await SaveTokenAsync(request, accessToken, cancellationToken);
                }

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

        private async Task SaveTokenAsync(TokenRequestData request, TokenModel? accessToken, CancellationToken cancellationToken)
        {
            var model = new TokenStoreModel($"R-{request.RequestId:N}", DateTime.UtcNow);
            model.RequestId = request.RequestId;

            model.Tokens.Add(new TokenInfo(TokenType.Access, accessToken!.Token!)
            {
                ExpiresAt = accessToken.ValidTo
            });

            await _authTokenStore.StoreAsync(model, cancellationToken);
        }
        
        public async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(
            string authority,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse disco = await httpClient
                .GetDiscoveryDocumentAsync(authority, cancellationToken);

            return disco;
        }
    }
}
