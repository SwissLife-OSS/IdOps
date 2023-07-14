using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IdOps;

public class ClientAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    

    public ClientAuthorizationMiddleware(
        RequestDelegate next,
        IHttpClientFactory httpClientFactory)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
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

        var clientId = routeData["id"]?.ToString();
        var secretId = Guid.Parse(context.Request.Form["secret"].ToString());
        var authority = context.Request.Form["authority"].ToString();
        var callbackUri = context.Request.Form["callback"].ToString();

        using HttpClient httpClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco = await httpClient.GetDiscoveryDocumentAsync(authority, CancellationToken.None);
        
        var uriBuilder = new UriBuilder(disco.AuthorizeEndpoint);
        var query = new Dictionary<string, string>();
        var codeVerifier = Pkce.GenerateCodeVerifier();
        
        query["response_type"] = "code";
        query["scope"] = "code";
        query["client_id"] = clientId;
        query["state"] = "code";
        query["redirect_uri"] = "http://localhost:5000/clients/callback";
        query["code_challenge"] = Pkce.GenerateCodeChallenge(codeVerifier);
        query["code_challenge_method"] = "S256";
        query["response_mode"] = "form_post";

        uriBuilder.Query = query.ToString();
        string url = uriBuilder.ToString();
        
        context.Response.Redirect(url);
        return;
    }
}