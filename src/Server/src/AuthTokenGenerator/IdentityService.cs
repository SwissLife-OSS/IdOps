using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps
{
    public class IdentityService : IIdentityService
    {
        private readonly ITokenClient _tokenClient;
        private readonly ITokenAnalyzer _tokenAnalyzer;

        public IdentityService(ITokenClient tokenClient, ITokenAnalyzer tokenAnalyzer)
        {
            _tokenClient = tokenClient;
            _tokenAnalyzer = tokenAnalyzer;
        }

        public async Task<RequestTokenResult> RequestTokenAsync(
            TokenRequest request,
            CancellationToken cancellationToken)
        {
            TokenResponse response = request.GrantType switch
            {
                "client_credentials" => await _tokenClient.RequestClientCredentialsTokenAsync(
                    (ClientCredentialsTokenRequest)request, cancellationToken),
                "authorization_code" => await _tokenClient.RequestAuthorizationCodeTokenAsync(
                    (AuthorizationCodeTokenRequest)request, cancellationToken),
                _ => await _tokenClient.RequestTokenAsync(request, cancellationToken)
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

            return new RequestTokenResult(true) { AccessToken = accessToken };
        }
    }
}
