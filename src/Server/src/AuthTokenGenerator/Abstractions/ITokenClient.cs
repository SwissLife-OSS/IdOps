using IdentityModel.Client;

namespace IdOps.Abstractions;

public interface ITokenClient
{
    Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(
        string address, 
        CancellationToken cancellationToken);

    Task<TokenResponse> RequestTokenAsync(
        TokenRequest request,
        CancellationToken cancellationToken);

    Task<TokenResponse> RequestClientCredentialsTokenAsync(
        ClientCredentialsTokenRequest request, 
        CancellationToken cancellationToken);
}
