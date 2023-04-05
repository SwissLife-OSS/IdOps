using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenAnalyzer _tokenAnalyzer;
        private readonly IAuthTokenStore _authTokenStore;

        public IdentityService(
            IHttpClientFactory httpClientFactory,
            ITokenAnalyzer tokenAnalyzer,
            IAuthTokenStore authTokenStore)
        {
            _httpClientFactory = httpClientFactory;
            _tokenAnalyzer = tokenAnalyzer;
            _authTokenStore = authTokenStore;
        }

        public async Task<UserInfoResult> GetUserInfoAsync(
            string token,
            CancellationToken cancellationToken)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt = handler.ReadToken(token) as JwtSecurityToken;

            if (jwt is { })
            {
                var issuer = jwt.Claims.Single(x => x.Type == "iss").Value;

                var client = new HttpClient();
                UserInfoResponse? response = await client.GetUserInfoAsync(
                    new UserInfoRequest
                    {
                        Address = issuer.Trim('/') + "/connect/userinfo",
                        Token = token
                    }, cancellationToken);

                return new UserInfoResult(response.Error)
                {
                    Claims = response.Claims.Select(x => new UserClaim(x.Type, x.Value))
                };
            }

            return new UserInfoResult("InvalidToken");
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

        public async Task<IEnumerable<TokenInfo>> RefreshTokenAsync(
            IdentityRequestItemData identityRequest,
            string refreshToken,
            CancellationToken cancellationToken)
        {
            var tokens = new List<TokenInfo>();

            HttpClient? httpClient = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse? disco = await GetDiscoveryDocumentAsync(
                identityRequest.Authority,
                cancellationToken);

            TokenResponse tokenResponse = await httpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = identityRequest.ClientId,
                    ClientSecret = identityRequest.Secret,
                    RefreshToken = refreshToken
                }, cancellationToken);

            if (tokenResponse.IsError)
            {
                throw new ApplicationException(
                    $"Could not refresh token. {tokenResponse.Error}");
            }

            tokens.Add(new TokenInfo(TokenType.Refresh, tokenResponse.RefreshToken));
            tokens.Add(new TokenInfo(TokenType.Access, tokenResponse.AccessToken)
            {
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            });

            if (tokenResponse.IdentityToken is { })
            {
                tokens.Add(new TokenInfo(TokenType.Id, tokenResponse.IdentityToken));
            }

            return tokens;
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
