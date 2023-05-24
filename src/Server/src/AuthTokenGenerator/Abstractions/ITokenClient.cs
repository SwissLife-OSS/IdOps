using IdentityModel.Client;

namespace IdOps.Abstractions;

public interface ITokenClient
{
    Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(
        string address = null, 
        CancellationToken cancellationToken = default);

    Task<TokenResponse> RequestTokenAsync(
        TokenRequest request,
        CancellationToken cancellationToken = default);

    Task<TokenResponse> RequestClientCredentialsTokenAsync(
        ClientCredentialsTokenRequest request, 
        CancellationToken cancellationToken = default);
}
