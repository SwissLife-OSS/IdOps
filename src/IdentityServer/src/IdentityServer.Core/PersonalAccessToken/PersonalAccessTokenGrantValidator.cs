using System;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using FluentValidation.Results;
using IdOps.IdentityServer.Events;
using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer
{
    public class PersonalAccessTokenGrantValidator : IExtensionGrantValidator
    {
        private readonly IPersonalAccessTokenValidator _patValidator;
        private readonly IEventService _eventService;

        public string GrantType => Wellknown.GrantTypes.PersonalAccessToken;

        public PersonalAccessTokenGrantValidator(
            IPersonalAccessTokenValidator patValidator,
            IEventService eventService)
        {
            _patValidator = patValidator;
            _eventService = eventService;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            string? token = context.Request.Raw["token"];
            context.Request.UserName = context.Request.Raw["username"];
            string? userName = context.Request.UserName;

            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(token))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);

                    await PersonalAccessTokenValidationFailedEvent
                        .New(
                            context.Request.ClientId,
                            userName,
                            "The username or Token seems to be empty or white space",
                            context.Request.RequestedScopes,
                            null)
                        .RaiseAsync(_eventService);

                    return;
                }

                PersonalAccessTokenValidationContext validationContext =
                    PersonalAccessTokenValidationContext.From(context, token);

                PersonalAccessTokenValidationResult patValidationResult =
                    await _patValidator.ValidateAsync(validationContext);

                if (!patValidationResult.IsValid)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);

                    await PersonalAccessTokenValidationFailedEvent
                        .New(
                            context.Request.ClientId,
                            userName,
                            "The access token validation failed",
                            context.Request.RequestedScopes,
                            null)
                        .RaiseAsync(_eventService);

                    return;
                }

                await PersonalAccessTokenValidationSuccessEvent
                    .New(
                        context.Request.ClientId,
                        userName,
                        string.Join(",", validationContext.RequestedScopes))
                    .RaiseAsync(_eventService);

                context.Result = patValidationResult.GrantValidationResult;
            }
            catch (Exception)
            {
                await PersonalAccessTokenValidationFailedEvent
                    .New(
                        context.Request.ClientId,
                        context.Request.UserName,
                        "Unexpected exception during PAT validation",
                        context.Request.RequestedScopes,
                        Array.Empty<ValidationFailure>())
                    .RaiseAsync(_eventService);
                throw;
            }
        }
    }
}
