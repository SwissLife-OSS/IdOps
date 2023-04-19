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
        private readonly IAuthTokenStore _authTokenStore;

        public IdentityService(
            IHttpClientWrapper httpClientWrapper,
            ITokenAnalyzer tokenAnalyzer,
            IAuthTokenStore authTokenStore)
        {
            _httpClientWrapper = httpClientWrapper;
            _tokenAnalyzer = tokenAnalyzer;
            _authTokenStore = authTokenStore;
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
    }
}
