using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using static Microsoft.AspNetCore.Routing.Patterns.RoutePatternFactory;

namespace IdOps;

public static class ClientAppAuthorizationEndpointRouteBuilder
{
    public static IEndpointRouteBuilder MapAuthorizeClient(
        this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapAuthorizeClientAuth()
            .MapAuthorizeClientAuthCallback();

        return endpointRouteBuilder;
    }

    private static IEndpointRouteBuilder MapAuthorizeClientAuthCallback(
        this IEndpointRouteBuilder endpointRouteBuilder)
    {
        RoutePattern pattern = Parse("clients/callback");
        IApplicationBuilder requestPipeline = endpointRouteBuilder
            .CreateApplicationBuilder();

        requestPipeline
            .UseMiddleware<ClientAuthorizationCallbackMiddleware>()
            .Use(_ => context =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            });

        endpointRouteBuilder
            .Map(pattern, requestPipeline.Build())
            .WithDisplayName("Clients-authorize-callback")
            .AllowAnonymous();

        return endpointRouteBuilder;
    }

    private static IEndpointRouteBuilder MapAuthorizeClientAuth(
        this IEndpointRouteBuilder endpointRouteBuilder)
    {
        RoutePattern pattern = Parse("clients/{id:guid}");
        IApplicationBuilder requestPipeline = endpointRouteBuilder
            .CreateApplicationBuilder();

        requestPipeline
            .UseMiddleware<ClientAuthorizationMiddleware>()
            .Use(_ => context =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            });

        endpointRouteBuilder
            .Map(pattern, requestPipeline.Build())
            .WithDisplayName("Clients-authorize")
            .AllowAnonymous();

        return endpointRouteBuilder;
    }
}