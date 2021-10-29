using IdOps.Api.Security;
using Microsoft.AspNetCore.Builder;

namespace IdOps.AspNet
{
    public static class IdOpsMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdOps(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<EnsureAuthenticatedMiddleware>()
                .UseMiddleware<UserContextMiddleware>();
        }
    }
}
