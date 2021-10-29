using Microsoft.AspNetCore.Authentication;

namespace IdOps.Api.Security
{
    public static partial class AuthenticationExtensions
    {
        static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder)
        {
            builder.AddDevToken();
        }
    }
}
