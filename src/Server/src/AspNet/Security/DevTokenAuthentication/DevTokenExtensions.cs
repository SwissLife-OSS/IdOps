using System;
using Microsoft.AspNetCore.Authentication;

namespace IdOps.Api.Security
{
    public static class DevTokenExtensions
    {
        public static AuthenticationBuilder AddDevToken(this AuthenticationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .AddScheme<DevTokenAuthenticationOptions, DevTokenAuthorizationHandler>(
                    DevTokenDefaults.AuthenticationScheme,
                    DevTokenDefaults.AuthenticationScheme,
                    _ => {  });
        }
    }
}
