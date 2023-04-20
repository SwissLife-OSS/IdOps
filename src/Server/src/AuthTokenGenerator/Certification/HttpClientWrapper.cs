using IdentityModel.Client;
using IdOps.Abstractions;

namespace IdOps.Certification;

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpClientWrapper(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    public async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(
        string authority,
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco =
            await httpClient.GetDiscoveryDocumentAsync(authority, cancellationToken);

        return disco;
    }

    public async Task<TokenResponse> RequestTokenAsync(
        TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        var response =  await httpClient.RequestTokenAsync(request, cancellationToken);
        return response;
    }

    public async Task<TokenResponse> RequestClientCredentialsTokenAsync(
        ClientCredentialsTokenRequest request, 
        CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        var response =  await httpClient.RequestClientCredentialsTokenAsync(request, cancellationToken);
        return response;
    }
}
