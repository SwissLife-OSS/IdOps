using IdentityModel.Client;
using IdOps.Abstractions;

namespace IdOps.Certification;

public class TokenClient : ITokenClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TokenClient(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    public async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(
        string address,
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco =
            await httpClient.GetDiscoveryDocumentAsync(address, cancellationToken);

        return disco;
    }

    public async Task<TokenResponse> RequestTokenAsync(
        TokenRequest request,
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        var response =  await httpClient.RequestTokenAsync(request, cancellationToken);
        return response;
    }

    public async Task<TokenResponse> RequestClientCredentialsTokenAsync(
        ClientCredentialsTokenRequest request, 
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        var response =  await httpClient.RequestClientCredentialsTokenAsync(request, cancellationToken);
        return response;
    }
}
