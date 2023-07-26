using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdOps.Abstractions;
using IdOps.GraphQL;
using IdOps.Models;
using IdOps.Store;
using Microsoft.AspNetCore.Http;

namespace IdOps;

public class ClientAuthorizationCallbackMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISessionStore _sessionStore;
    private readonly IResultFactory<TokenRequest, RequestTokenInput> _tokenRequestFactory;
    private readonly IIdentityService _identityService;
    

    public ClientAuthorizationCallbackMiddleware(
        RequestDelegate next,
        ISessionStore sessionStore,
        IResultFactory<TokenRequest, RequestTokenInput> tokenRequestFactory,
        IIdentityService identityService
        )
    {
        _next = next;
        _sessionStore = sessionStore;
        _tokenRequestFactory = tokenRequestFactory;
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
        var issuer = context.Request.Form["iss"].ToString();
        Session session;
        try
        {
            session = _sessionStore.GetSession(stateId);
        }
        catch (KeyNotFoundException e)
        {
            await Task.Delay(1000);
            session = _sessionStore.GetSession(stateId);
        }

        
        var requestTokenInput 
            = new RequestAuthorizationCodeTokenInput(
                issuer, 
                session.ClientId,
                session.SecretId,
                "authorization_code",
                code,
                session.CodeVerifier,
                "http://localhost:5000/clients/callback"
                );

        TokenRequest request =
            await _tokenRequestFactory.CreateRequestAsync(requestTokenInput,
                CancellationToken.None);


        RequestTokenResult result = await _identityService.RequestTokenAsync(
            request, CancellationToken.None);
        
        context.Response.Redirect(session.CallbackUri);
        
        return;
    }
    
}


