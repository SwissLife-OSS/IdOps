using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace IdOps.Api.Security
{
    public static partial class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddAuthentication(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            SecurityOptions secOptions = configuration.GetSection("Security")
                .Get<SecurityOptions>();

            AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = env.IsDevelopment() ?
                    DevTokenDefaults.AuthenticationScheme :
                    CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = "oidc";
            });

            authBuilder.AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.Cookie.Name = "ops-id";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = secOptions.Authority;
                options.RequireHttpsMetadata = !env.IsDevelopment();
                options.ClientSecret = secOptions.ClientSecret;
                options.ClientId = secOptions.ClientId;
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.MapAllExcept("role", "user_application", "idp", "nbf", "iat", "exp", "amr", "nonce", "at_hash", "auth_time", "tenant");
                options.ClaimActions.MapUserRoles();

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = (ctx) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };
            });

            if (env.IsDevelopment())
            {
                SetupDevelopmentAuthentication(authBuilder);
            }

            return authBuilder;
        }

        static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder);
    }
}
