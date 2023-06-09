using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using Duende.IdentityServer.Services;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.IpWhitelist;

namespace IdOps.IdentityServer;

public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IpWhitelistValidator _validator;
    private readonly IEventService _eventService;
    private readonly ClientIdExtractor _clientIdExtractor;

    public IpWhitelistMiddleware(
        RequestDelegate next,
        IpWhitelistValidator validator,
        IEventService eventService,
        ClientIdExtractor clientIdExtractor)
    {
        _next = next;
        _validator = validator;
        _eventService = eventService;
        _clientIdExtractor = clientIdExtractor;
    }

    public async Task InvokeAsync(HttpContext context, IClientStore clientStore)
    {
        var clientId = _clientIdExtractor.GetClientId(context);

        if (!string.IsNullOrEmpty(clientId))
        {
            Client client = await clientStore.FindEnabledClientByIdAsync(clientId);

            if (client is IdOpsClient { IpAddressWhitelist: not null } idOpsClient)
            {
                if (!_validator.IsValid(idOpsClient, out var message))
                {
                    await IpValidationFailedEvent.New(clientId, message).RaiseAsync(_eventService);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
        }

        await _next(context);
    }
}
