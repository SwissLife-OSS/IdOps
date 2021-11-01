using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using FluentValidation;
using IdentityModel;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Model;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace IdOps.IdentityServer
{
    public class LocalAccessTokenSource : IPersonalAccessTokenSource
    {
        private readonly IEventService _eventService;

        public LocalAccessTokenSource(IEventService eventService)
        {
            _eventService = eventService;
        }

        public string Kind => "LOCAL";

        public async Task<PersonalAccessTokenValidationResult> ValidateAsync(
            PersonalAccessTokenValidationContext context,
            PersonalAccessTokenMatch pat)
        {
            ValidationContext<PersonalAccessTokenMatch> validationContext = new(pat)
            {
                RootContextData =
                {
                    ["scopes"] = context.RequestedScopes,
                    ["client"] = context.Client.ClientId
                }
            };

            LocalPersonalAccessTokenAuthenticationValidator validator = new();
            ValidationResult? validationResult =
                await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                await _eventService.RaiseAsync(
                    new PersonalAccessTokenValidationFailedEvent(
                        string.Empty,
                        pat.Definition.UserName,
                        "The validation failed, check the Errors",
                        string.Join(",", context.RequestedScopes),
                        validationResult.Errors));

                return PersonalAccessTokenValidationResult.Invalid;
            }

            string subject = pat.Definition.UserName;
            List<Claim> claims = new();
            foreach (ClaimExtension claimExtension in pat.Definition.ClaimExtensions)
            {
                if (claimExtension.Type == JwtClaimTypes.Subject)
                {
                    subject = claimExtension.Value;
                }

                claims.Add(new Claim(claimExtension.Type, claimExtension.Value));
            }

            return new PersonalAccessTokenValidationResult(
                new GrantValidationResult(
                    subject,
                    Wellknown.GrantTypes.PersonalAccessToken,
                    claims));
        }
    }
}
