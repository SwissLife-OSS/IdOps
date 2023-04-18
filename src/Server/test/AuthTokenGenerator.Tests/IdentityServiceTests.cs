using System.Net.Http.Json;
using System.Text.Json;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Models;
using Moq;
using Xunit;

namespace IdOps
{
    public class IdentityServiceTests
    {
        [Fact]
        public async Task RequestTokenAsync_WithValidRequest_ShouldReturnToken()
        {
            //Arrange
            var tokenRequestData = new TokenRequestData("http://localhost:1234",
                "11111111111111111111111111111111",
                "thisIsATestSecret" +
                "111111111111111111111111111111111111111111111111111111111111111111111",
                "client_credentials", new[] { "scope1", "scope2" },
                new[] { new TokenRequestParameter("test") });

            var tokenResponse = new DummyTokenResponse(JsonDocument.Parse(
                    "{\"access_token\": \"eyJhbGciOiJSUzI1NiIsImtpZCI6IjE1QTg3QThCNkRCMzZEQTE2OURCMDU1M0VERkYwOERCIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDEiLCJuYmYiOjE2ODEzNjY5MzksImlhdCI6MTY4MTM2NjkzOSwiZXhwIjoxNjgxMzcwNTM5LCJzY29wZSI6WyJhcGkucmVhZCJdLCJjbGllbnRfaWQiOiI3OWRjMTNmNDdmZDI0MTQ4YjRhYmE2ZTZlNzk3ODc2YSIsImp0aSI6IjU5NUVEMjhCODY3REJDRkQxQkY2RUZCOUY0REY5RkI0In0.VbCNgtoBCWpumUjSaMrdrAksyTuko7Np0ieAIfddrUKw6EDROqsyN2mtwQ5F2IFGiFjHRlrUQUHGxgxY-d4jR2tLz9N7mPMmXMYRX3Vsf14gULnGy7lcX2y3LCBmZPeLzf0MLDMeDScCWMUQCxI3ZynrGKNsqMLO5FOGNSDPDCRk7ig8NX34PfXZmqM_Wd8KSvOwQbMqTU2JtNu5DCoX9M36To_5W3otO-IPpQZMz8GkEVIyPlGjknlhA40XSo19FlsxNdLEnKQJYlMyI9WKpAdsbVYjgQqGHjzZaGpE2h1hz6EQX8yVUNQ4_u3XtninbTrPNftLvuQkj4DZtsdQrg\",  \"expires_in\": 3600,  \"token_type\": \"Bearer\",  \"scope\": \"api.read\"}")
                .RootElement);

            var httpClientMock = new Mock<HttpClient>(MockBehavior.Strict);

            httpClientMock.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), CancellationToken.None))
                .ReturnsAsync(new HttpResponseMessage(){RequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test"),Content = JsonContent.Create<string>("{\"test\": \"value\"}")});
            

            HttpClient httpClient = httpClientMock.Object;

            var httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);
            IHttpClientFactory httpClientFactory = httpClientFactoryMock.Object;

            var tokenAnalyzerMock = new Mock<ITokenAnalyzer>(MockBehavior.Strict);
            tokenAnalyzerMock.Setup(analyzer =>
                    analyzer.Analyze(It.Is<string>(s => s.Equals(tokenResponse.AccessToken))))
                .Returns(new TokenModel());
            ITokenAnalyzer tokenAnalyzer = tokenAnalyzerMock.Object;

            var authTokenStoreMock = new Mock<IAuthTokenStore>(MockBehavior.Strict);
            IAuthTokenStore authTokenStore = authTokenStoreMock.Object;


            var identityService =
                new IdentityService(httpClientFactory, tokenAnalyzer, authTokenStore);

            //Act
            RequestTokenResult _ =
                await identityService.RequestTokenAsync(tokenRequestData, CancellationToken.None);

            //Assert
            httpClientMock.VerifyAll();
            httpClientFactoryMock.VerifyAll();
            tokenAnalyzerMock.VerifyAll();
            authTokenStoreMock.VerifyAll();
        }
    }

    class DummyTokenResponse : TokenResponse
    {
        public DummyTokenResponse(JsonElement jsonElement)
        {
            Json = jsonElement;
        }
    }

    interface IDummyHttpClinetDiscoveryExtensions
    {
        static Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(HttpClient client, string address = null, CancellationToken cancellationToken = default)
    }
}
