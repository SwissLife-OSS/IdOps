using IdOps.IdentityServer.Model;
using IdOps.IdentityServer;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Net;

namespace IdOps;

public class IpAllowListValidatorTests
{
    private static Mock<IHttpContextAccessor> SetupMockHttpContextAccessor(
        string? xForwardedFor,
        string? remoteIpAddress = null)
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpRequest = new Mock<HttpRequest>();
        var mockConnection = new Mock<ConnectionInfo>();

        
        mockHttpRequest.Setup(req => req.Headers["X-Forwarded-For"]).Returns(xForwardedFor);

        if (remoteIpAddress != null)
        {
            mockConnection.Setup(conn => conn.RemoteIpAddress)
                .Returns(IPAddress.Parse(remoteIpAddress));
        }

        mockHttpContext.Setup(ctxt => ctxt.Request).Returns(mockHttpRequest.Object);
        mockHttpContext.Setup(ctxt => ctxt.Connection).Returns(mockConnection.Object);
        mockHttpContextAccessor.Setup(ac => ac.HttpContext).Returns(mockHttpContext.Object);
        return mockHttpContextAccessor;
    }

    [Fact]
    public void GivenNoWhitelist_WhenIsValidCalled_ThenReturnTrue()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1");

        var client = new IpAddressFilter
        {
            Policy = IpFilterPolicy.Public
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenWhitelistWithIncomingIP_WhenIsValidCalled_ThenReturnTrue()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.1" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenWhitelistWithoutIncomingIP_WhenIsValidCalled_ThenReturnFalseWithCorrectErrorMessage()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.2", "192.168.1.3" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out string message);

        // Assert
        Assert.False(result);
        Assert.Equal("192.168.1.1 was not part of whitelist 192.168.1.2, 192.168.1.3", message);
    }

    [Fact]
    public void GivenEmptyClientIpWhitelist_WhenIsValidCalled_ThenReturnFalse()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1");

        var client = new IpAddressFilter
        {
            AllowList = new List<string>(),
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GivenInvalidIncomingIP_WhenIsValidCalled_ThenReturnFalseWithCorrectErrorMessage()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor(null);

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.2", "192.168.1.3" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out string message);

        // Assert
        Assert.False(result);
        Assert.Equal("IP address was not present in request", message);
    }

    [Fact]
    public void GivenInvalidWhitelistIPs_WhenIsValidCalled_ThenConsiderOnlyValidIPs()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.2");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.2", "InvalidIPAddress" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenXForwardedForMultipleIPs_WhenIsValidCalled_ThenReturnLeftmostIP()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1, 192.168.1.2");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.1" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenXForwardedForSingleIP_WhenIsValidCalled_ThenReturnThatIP()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.1" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenXForwardedForEmpty_WhenIsValidCalled_ThenReturnRemoteIpAddress()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor(null, "192.168.1.1");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.1" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GivenWhitelistWithIncomingIPWithPort_WhenIsValidCalled_ThenReturnTrue()
    {
        // Arrange
        Mock<IHttpContextAccessor> mockHttpContextAccessor = SetupMockHttpContextAccessor("192.168.1.1:8080");

        var client = new IpAddressFilter
        {
            AllowList = new List<string> { "192.168.1.1" },
            Policy = IpFilterPolicy.AllowList            
        };

        var validator = new IpAllowListValidator(mockHttpContextAccessor.Object, new InternalIpFilterConfiguration());

        // Act
        var result = validator.IsValid(client, out _);

        // Assert
        Assert.True(result);
    }

    // TODO: Add test for Internal Policy
}
