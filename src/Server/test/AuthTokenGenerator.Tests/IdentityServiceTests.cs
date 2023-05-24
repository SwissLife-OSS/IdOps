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
        private Mock<ITokenClient> _httpClientWrapperMock;
        private Mock<ITokenAnalyzer> _tokenAnalyzerMock;

        [Theory]
        [InlineData("client_credentials", 0, 1)]
        [InlineData("foobar", 1, 0)]
        public async Task RequestTokenAsync_WithValidRequest_ShouldReturnToken(
            string grantType,
            int timesTokenRequest, 
            int timesCredentialsRequest)
        {
            //Arrange
            initializeMocks();

            //Setup httpClientWrapperMock
            _httpClientWrapperMock
                .Setup(wrapper =>
                    wrapper.RequestTokenAsync(It.IsAny<TokenRequest>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(CreateToken);

            _httpClientWrapperMock
                .Setup(wrapper =>
                    wrapper.GetDiscoveryDocumentAsync(It.IsAny<string>(),
                        It.IsAny<CancellationToken>())).ReturnsAsync(CreateDiscoveryResponse());

            _httpClientWrapperMock
                .Setup(wrapper => wrapper.RequestClientCredentialsTokenAsync(
                    It.IsAny<ClientCredentialsTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateClientCredentialsResponse);

            //Setup tokenAnalyzerMock
            _tokenAnalyzerMock.Setup(analyzer => analyzer.Analyze(It.IsAny<string>()))
                .Returns(new TokenModel());

            _tokenAnalyzerMock.Setup(analyzer => analyzer.Analyze(It.IsAny<string>()))
                .Returns(new TokenModel());

            //Setup identityService
            var identityService = new IdentityService(_httpClientWrapperMock.Object,
                _tokenAnalyzerMock.Object);

            //Act
            RequestTokenResult _ =
                await identityService.RequestTokenAsync(CreateTokenRequestData(grantType),
                    CancellationToken.None);

            //Assert
            _httpClientWrapperMock.Verify(
                wrapper => wrapper.RequestClientCredentialsTokenAsync(
                    It.IsAny<ClientCredentialsTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(timesCredentialsRequest));
            _httpClientWrapperMock.Verify(
                wrapper => wrapper.RequestTokenAsync(
                    It.IsAny<TokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(timesTokenRequest));
            _tokenAnalyzerMock.VerifyAll();
        }

        private void initializeMocks()
        {
            _httpClientWrapperMock = new Mock<ITokenClient>(MockBehavior.Strict);
            _tokenAnalyzerMock = new Mock<ITokenAnalyzer>(MockBehavior.Strict);
        }

        private TokenRequestData CreateTokenRequestData(string grantType)
        {
            var tokenRequestData = new TokenRequestData("http://localhost:1234",
                "11111111111111111111111111111111",
                "thisIsATestSecret" +
                "111111111111111111111111111111111111111111111111111111111111111111111", grantType,
                new[] { "scope1", "scope2" }, new[] { new TokenRequestParameter("test") });
            return tokenRequestData;
        }

        private DiscoveryDocumentResponse CreateDiscoveryResponse()
        {
            var discoveryResponse = new DummyDiscoveryDocument(JsonDocument
                .Parse("{\"issuer\":\"http://localhost:5001\"," +
                       "\"jwks_uri\":\"http://localhost:5001/.well-known/openid-configuration/jwks\"," +
                       "\"authorization_endpoint\":\"http://localhost:5001/connect/authorize\"," +
                       "\"token_endpoint\":\"http://localhost:5001/connect/token\"," +
                       "\"userinfo_endpoint\":\"http://localhost:5001/connect/userinfo\"," +
                       "\"end_session_endpoint\":\"http://localhost:5001/connect/endsession\"," +
                       "\"check_session_iframe\":\"http://localhost:5001/connect/checksession\"," +
                       "\"revocation_endpoint\":\"http://localhost:5001/connect/revocation\"," +
                       "\"introspection_endpoint\":\"http://localhost:5001/connect/introspect\"," +
                       "\"device_authorization_endpoint\":\"http://localhost:5001/connect/deviceauthorization\"," +
                       "\"backchannel_authentication_endpoint\":\"http://localhost:5001/connect/ciba\"," +
                       "\"frontchannel_logout_supported\":true," +
                       "\"frontchannel_logout_session_supported\":true," +
                       "\"backchannel_logout_supported\":true," +
                       "\"backchannel_logout_session_supported\":true," +
                       "\"scopes_supported\":[\"openid\",\"api.read\",\"offline_access\"]," +
                       "\"claims_supported\":[\"sub\"]," +
                       "\"grant_types_supported\":[\"authorization_code\",\"client_credentials\",\"refresh_token\",\"implicit\",\"urn:ietf:params:oauth:grant-type:device_code\",\"urn:openid:params:grant-type:ciba\",\"personal_access_token\"]," +
                       "\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"id_token token\",\"code id_token\",\"code token\",\"code id_token token\"]," +
                       "\"response_modes_supported\":[\"form_post\",\"query\",\"fragment\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_basic\",\"client_secret_post\"]," +
                       "\"id_token_signing_alg_values_supported\":[\"RS256\"]," +
                       "\"subject_types_supported\":[\"public\"],\"" +
                       "code_challenge_methods_supported\":[\"plain\",\"S256\"]," +
                       "\"request_parameter_supported\":true," +
                       "\"request_object_signing_alg_values_supported\":[\"RS256\",\"RS384\",\"RS512\",\"PS256\",\"PS384\",\"PS512\",\"ES256\",\"ES384\",\"ES512\",\"HS256\",\"HS384\",\"HS512\"]," +
                       "\"authorization_response_iss_parameter_supported\":true,\"backchannel_token_delivery_modes_supported\":[\"poll\"],\"backchannel_user_code_parameter_supported\":true}")
                .RootElement);

            return discoveryResponse;
        }

        private TokenResponse CreateToken()
        {
            var token = new DummyTokenResponse(JsonDocument.Parse(
                    "{\"issuer\":\"http://localhost:5001\"," +
                    "\"jwks_uri\":\"http://localhost:5001/.well-known/openid-configuration/jwks\"," +
                    "\"authorization_endpoint\":\"http://localhost:5001/connect/authorize\"," +
                    "\"token_endpoint\":\"http://localhost:5001/connect/token\"," +
                    "\"userinfo_endpoint\":\"http://localhost:5001/connect/userinfo\"," +
                    "\"end_session_endpoint\":\"http://localhost:5001/connect/endsession\"," +
                    "\"check_session_iframe\":\"http://localhost:5001/connect/checksession\"," +
                    "\"revocation_endpoint\":\"http://localhost:5001/connect/revocation\"," +
                    "\"introspection_endpoint\":\"http://localhost:5001/connect/introspect\"," +
                    "\"device_authorization_endpoint\":\"http://localhost:5001/connect/deviceauthorization\"," +
                    "\"backchannel_authentication_endpoint\":\"http://localhost:5001/connect/ciba\"," +
                    "\"frontchannel_logout_supported\":true," +
                    "\"frontchannel_logout_session_supported\":true," +
                    "\"backchannel_logout_supported\":true," +
                    "\"backchannel_logout_session_supported\":true," +
                    "\"scopes_supported\":[\"openid\",\"api.read\",\"offline_access\"]," +
                    "\"claims_supported\":[\"sub\"]," +
                    "\"grant_types_supported\":[\"authorization_code\",\"client_credentials\",\"refresh_token\",\"implicit\",\"urn:ietf:params:oauth:grant-type:device_code\",\"urn:openid:params:grant-type:ciba\",\"personal_access_token\"]," +
                    "\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"id_token token\",\"code id_token\",\"code token\",\"code id_token token\"]," +
                    "\"response_modes_supported\":[\"form_post\",\"query\",\"fragment\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_basic\",\"client_secret_post\"]," +
                    "\"id_token_signing_alg_values_supported\":[\"RS256\"]," +
                    "\"subject_types_supported\":[\"public\"],\"" +
                    "code_challenge_methods_supported\":[\"plain\",\"S256\"]," +
                    "\"request_parameter_supported\":true," +
                    "\"request_object_signing_alg_values_supported\":[\"RS256\",\"RS384\",\"RS512\",\"PS256\",\"PS384\",\"PS512\",\"ES256\",\"ES384\",\"ES512\",\"HS256\",\"HS384\",\"HS512\"]," +
                    "\"authorization_response_iss_parameter_supported\":true,\"backchannel_token_delivery_modes_supported\":[\"poll\"],\"backchannel_user_code_parameter_supported\":true}")
                .RootElement);
            return token;
        }

        private TokenResponse CreateClientCredentialsResponse()
        {
            var clientCredentialToken = new DummyTokenResponse(JsonDocument.Parse(
                "{\"access_token\": \"eyJhbGciOiJSUzI1NiIsImtpZCI6IjE1QTg3QThCNkRCMzZEQTE2OURCMDU1M0VERkYwOERCIiwidHlwIjoiYXQrand0In0." +
                "eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDEiLCJuYmYiOjE2ODEzNjY5MzksImlhdCI6MTY4MTM2NjkzOSwiZXhwIjoxNjgxMzcwNTM5LCJzY29wZSI6WyJhcGkucmVhZCJdLCJjbGllbnRfaWQiOiI3" +
                "OWRjMTNmNDdmZDI0MTQ4YjRhYmE2ZTZlNzk3ODc2YSIsImp0aSI6IjU5NUVEMjhCODY3REJDRkQxQkY2RUZCOUY0REY5RkI0In0." +
                "VbCNgtoBCWpumUjSaMrdrAksyTuko7Np0ieAIfddrUKw6EDROqsyN2mtwQ5F2IFGiFjHRlrUQUHGxgxY-d4jR2tLz9N7mPMmXMYRX3Vsf14gULnGy7lcX2y3LCBmZ" +
                "PeLzf0MLDMeDScCWMUQCxI3ZynrGKNsqMLO5FOGNSDPDCRk7ig8NX34PfXZmqM_Wd8KSvOwQbMqTU2JtNu5DCoX9M36To_5W3otO-IPpQZMz8GkEVIyPlGjknlhA40XSo19FlsxNdLEnKQJYlMyI9WKpAdsbVYjgQqGHjzZaGpE2h1hz" +
                "6EQX8yVUNQ4_u3XtninbTrPNftLvuQkj4DZtsdQrg\"," + "  \"expires_in\": 3600," +
                "  \"token_type\": \"Bearer\"," + "  \"scope\": \"api.read\"}").RootElement);
            return clientCredentialToken;
        }

        class DummyTokenResponse : TokenResponse
        {
            public DummyTokenResponse(JsonElement jsonElement)
            {
                Json = jsonElement;
            }
        }


        class DummyDiscoveryDocument : DiscoveryDocumentResponse
        {
            public DummyDiscoveryDocument(JsonElement jsonElement)
            {
                Json = jsonElement;
            }
        }
    }
}
