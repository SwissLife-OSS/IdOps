using System.IO;
using System.Text;
using IdOps.IdentityServer.IpWhitelist;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdOps;

public class ClientIdExtractorTests
{
    [Fact]
    public void HttpContextWithAuthorizationHeader_GetClientIdIsCalled_ClientIdIsReturned()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authValue = "Basic Y2xpZW50X2lkOmNsaWVudF9zZWNyZXQ=";
        context.Request.Headers["Authorization"] = authValue;
        var extractor = new ClientIdExtractor();

        // Act
        var result = extractor.GetClientId(context);

        // Assert
        Assert.Equal("client_id", result);
    }

    [Fact]
    public void HttpContextWithoutAuthorizationHeader_GetClientIdIsCalled_EmptyStringIsReturned()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var extractor = new ClientIdExtractor();

        // Act
        var result = extractor.GetClientId(context);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void HttpContextWithFormContentType_GetClientIdIsCalled_FormClientIdIsReturned()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/x-www-form-urlencoded";
        var body = Encoding.UTF8.GetBytes(
            "grant_type=client_credentials&scope=api1.read&client_id=test_client_id");
        context.Request.Body = new MemoryStream(body);

        var extractor = new ClientIdExtractor();

        // Act
        var result = extractor.GetClientId(context);

        // Assert
        Assert.Equal("test_client_id", result);
    }

    [Fact]
    public void HttpContextWithoutClientId_GetClientIdIsCalled_EmptyStringIsReturned()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/x-www-form-urlencoded";
        var body = Encoding.UTF8.GetBytes("grant_type=client_credentials&scope=api1.read");
        context.Request.Body = new MemoryStream(body);

        var extractor = new ClientIdExtractor();

        // Act
        var result = extractor.GetClientId(context);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
