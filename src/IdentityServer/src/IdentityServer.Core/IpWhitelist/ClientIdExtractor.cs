using System.Linq;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;

namespace IdOps.IdentityServer.IpWhitelist;

public class ClientIdExtractor
{
    private static readonly string BasicPrefix = "Basic ";
    private static readonly string ClientId = "client_id";

    public string? GetClientId(HttpContext context)
    {
        var clientId = string.Empty;

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith(
                BasicPrefix,
                StringComparison.InvariantCultureIgnoreCase))
        {
            var encodedUsernamePassword = authHeader[BasicPrefix.Length..].Trim();
            var decodedUsernamePassword =
                Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            clientId = decodedUsernamePassword.Split(':').FirstOrDefault();
        }

        if (string.IsNullOrEmpty(clientId) && context.Request.HasFormContentType)
        {
            if (context.Request.Form.ContainsKey(ClientId))
            {
                clientId = context.Request.Form[ClientId];
            }
        }

        return clientId;
    }
}
