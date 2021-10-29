using Microsoft.AspNetCore.Builder;

namespace IdOps.AspNet
{
    public static class EmbeddedUIMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdOpsUI(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EmbeddedUIMiddleware>();
        }
    }
}
