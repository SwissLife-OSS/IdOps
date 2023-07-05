using System.Security.Cryptography;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IdOps;

public class ClientAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    

    public ClientAuthorizationMiddleware(
        RequestDelegate next)
    {
        _next = next;
        
    }

    public async Task Invoke(HttpContext context)
    {
        Endpoint? endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        RouteValueDictionary routeData = context.GetRouteData().Values;

        var id = Guid.Parse(routeData["id"].ToString());

        
        


        //await _next(context);

        return;
    }

    private string BuildRedirectUri(HttpContext context)
    {
        return context.Request.Scheme + "://" + context.Request.Host.ToString() + "/clients/callback";
    }
}