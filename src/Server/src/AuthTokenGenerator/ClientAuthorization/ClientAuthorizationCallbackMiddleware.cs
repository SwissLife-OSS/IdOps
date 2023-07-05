using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.Model;
using IdOps.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace IdOps;

public class ClientAuthorizationCallbackMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IClientService _clientService;
    private readonly IIdentityService _identityService;
    

    public ClientAuthorizationCallbackMiddleware(
        RequestDelegate next,
        IClientService clientService,
        IIdentityService identityService
        )
    {
        _next = next;
        _clientService = clientService;
        _identityService = identityService;
    }

    public async Task Invoke(HttpContext context)
    {
        Endpoint? endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var stateId = context.Request.Form["state"].ToString();
        var code = context.Request.Form["code"].ToString();

        string delimiter = "://";
        string clientId = stateId.Substring(stateId.IndexOf(delimiter) + delimiter.Length);

        Client client = await _clientService.GetByIdAsync(new Guid(clientId),CancellationToken.None);

        RequestTokenResult result =
            await _identityService.RequestTokenAsync(new TokenRequestData
                (
                    "http://localhost:5001",
                    clientId,
                    "secret",
                    "authorization_code",
                    new []{"scope"}
                ),
                CancellationToken.None);



        return;
    }
    
}

public static class AzureADIssuerValidation
{
    internal static string Validate(
        string issuer,
        SecurityToken token,
        TokenValidationParameters parameters)
    {
        if (token is JwtSecurityToken jwt)
        {
            if (jwt.Payload.TryGetValue("tid", out var value) &&
                value is string tokenTenantId)
            {
                IEnumerable<string> validIssuers = (parameters.ValidIssuers ?? Enumerable.Empty<string>())
                    .Append(parameters.ValidIssuer)
                    .Where(i => !string.IsNullOrEmpty(i));

                if (validIssuers.Any(i => i.Replace("{tenantid}", tokenTenantId) == issuer))
                    return issuer;
            }
        }

        throw new SecurityTokenInvalidIssuerException("Bla")
        {
            InvalidIssuer = "Foo"
        };
    }

}
