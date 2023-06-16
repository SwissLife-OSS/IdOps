using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IdOps.IdentityServer.Model;
using Microsoft.AspNetCore.Http;

namespace IdOps.IdentityServer;

public class IpAllowListValidator
{
    private readonly IHttpContextAccessor _accessor;
    private readonly InternalIpFilterConfiguration _configuration;

    public IpAllowListValidator(
        IHttpContextAccessor accessor,
        InternalIpFilterConfiguration configuration)
    {
        _accessor = accessor;
        _configuration = configuration;
    }

    public bool IsValid(IpAddressFilter filter, out string message)
    {
        message = string.Empty;

        if (filter.Policy == IpFilterPolicy.Public)
        {
            return true;
        }

        IPAddress? ipAddress = GetIncomingIpAddress();

        if (ipAddress is null)
        {
            message = "IP address was not present in request";
            return false;
        }

        IList<IPAddress> allowList = GetAllowListFromFilter(filter);

        if (allowList.Contains(ipAddress))
        {
            return true;
        }

        message = $"{ipAddress} was not part of allow list {string.Join(", ", allowList)}";
        return false;
    }

    private IPAddress? GetIncomingIpAddress()
    {
        string? ipAddress = _accessor.HttpContext?.Request.Headers["X-Forwarded-For"];

        if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(
                ',',
                StringComparison.InvariantCultureIgnoreCase))
        {
            // X-Forwarded-For can contain multiple IP addresses when multiple proxies are involved
            // The leftmost IP address is the IP address of the originating client
            ipAddress = ipAddress.Split(',').First();
        }

        if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(
                ':',
                StringComparison.InvariantCultureIgnoreCase))
        {
            ipAddress = ipAddress.Split(':').First();
        }

        return ipAddress is null
            ? _accessor.HttpContext?.Connection.RemoteIpAddress
            : IPAddress.Parse(ipAddress);
    }

    private IList<IPAddress> GetAllowListFromFilter(IpAddressFilter filter)
    {
        return filter.Policy switch
        {
            IpFilterPolicy.Internal => _configuration.InternalIpAllowListParsed.Value,
            IpFilterPolicy.AllowList => filter.AllowList.Select(
                    a => IPAddress.TryParse(a, out IPAddress? ipAddress) ? ipAddress : null)
                .OfType<IPAddress>().ToList(),
            _ => throw new InvalidOperationException(
                $"There is no allowList for IpFilterPolicy: {filter.Policy}")
        };
    }
}
