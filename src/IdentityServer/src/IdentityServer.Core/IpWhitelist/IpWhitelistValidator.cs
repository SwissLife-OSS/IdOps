using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IdOps.IdentityServer.Model;
using Microsoft.AspNetCore.Http;

namespace IdOps.IdentityServer;

public class IpWhitelistValidator
{
    private readonly IHttpContextAccessor _accessor;

    public IpWhitelistValidator(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public bool IsValid(IdOpsClient client, out string message)
    {
        message = string.Empty;

        if (client.IpAddressWhitelist == null)
        {
            return true;
        }

        IPAddress? ipAddress = GetIncomingIpAddress();

        if (ipAddress is null)
        {
            message = "IP address was not present in request";
            return false;
        }

        List<IPAddress> whitelist = GetWhitelistFromClient(client);

        if (whitelist.Contains(ipAddress))
        {
            return true;
        }

        message = $"{ipAddress} was not part of whitelist {string.Join(", ", whitelist)}";
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

    private List<IPAddress> GetWhitelistFromClient(IdOpsClient client)
    {
        return client.IpAddressWhitelist?.Select(
                a => IPAddress.TryParse(a, out IPAddress? ipAddress) ? ipAddress : null)
            .OfType<IPAddress>().ToList() ?? new List<IPAddress>();
    }
}
