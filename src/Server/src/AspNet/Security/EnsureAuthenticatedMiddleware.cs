using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace IdOps.Api.Security
{
    public class EnsureAuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public EnsureAuthenticatedMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/_health") ||
                context.Request.Path.StartsWithSegments("/api/session") ||
                _env.IsDevelopment())
            {
                await _next(context);

                return;
            }

            if (context.Request.Path.StartsWithSegments("/api")
                || context.Request.Path.StartsWithSegments("/graphql")
                || context.Request.Path.StartsWithSegments("/signalR")
                || context.Request.Path.StartsWithSegments("/error"))
            {
                if (HasIdOpsRole(context))
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access denied!");
                }
            }
            else if (!context.User.Identity.IsAuthenticated)
            {
                if (context.Request.Path == "/")
                {
                    await context.ChallengeAsync();
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access denied!");
                }
            }
            else
            {
                await _next(context);
            }
        }

        private bool HasIdOpsRole(HttpContext context)
        {
            return context.User.Claims
                .Where(x => x.Type == "role" && x.Value.StartsWith("IdOps."))
                .Any();
        }
    }
}
