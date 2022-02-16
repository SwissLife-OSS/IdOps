using System;
using System.Collections.Generic;
using FluentValidation;
using Duende.IdentityServer.Extensions;

namespace IdOps.IdentityServer
{
    public abstract class PersonalAccessTokenAuthenticationValidator
        : AbstractValidator<PersonalAccessTokenMatch>
    {
        protected PersonalAccessTokenAuthenticationValidator()
        {
            AddDefinitionRules();
            AddTokenRules();
        }

        private void AddDefinitionRules()
        {
            RuleFor(x => x.Definition.UserName)
                .NotEmpty()
                .WithMessage("The UserName is mandatory.");

            RuleFor(x => x.Definition.CreatedAt)
                .NotEmpty()
                .WithMessage("The Created At date is required.");

            RuleFor(x => x.Definition.CreatedAt)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("The Creation date must be smaller or equal than the current date");

            RuleFor(x => x.Definition.AllowedScopes)
                .Custom((allowedScopes, context) =>
                {
                    IDictionary<string, object> contextData = context.RootContextData;

                    if (contextData.TryGetValue("scopes", out var rawScopes) &&
                        rawScopes is IEnumerable<string> scopes)
                    {
                        foreach (var scope in scopes)
                        {
                            if (allowedScopes.IsNullOrEmpty() || !allowedScopes.Contains(scope))
                            {
                                context.AddFailure("The scope is not allowed for the current token.");
                            }
                        }
                    }
                    else
                    {
                        context.AddFailure("No scopes provided.");
                    }
                });

            RuleFor(x => x.Definition.AllowedClients)
                .Custom((allowedClients, context) =>
                {
                    IDictionary<string, object> contextData = context.RootContextData;

                    if (contextData.TryGetValue("client", out var rawClient) &&
                        rawClient is string client &&
                        !string.IsNullOrEmpty(client))
                    {
                        if (allowedClients.IsNullOrEmpty() || !allowedClients.Contains(client))
                        {
                            context.AddFailure("The client is not allowed for the current token");
                        }
                    }
                    else
                    {
                        context.AddFailure("No client provided.");
                    }
                });
        }

        private void AddTokenRules()
        {
            RuleFor(x => x.Token.Token)
                .NotEmpty()
                .WithMessage("The Token is mandatory.");

            RuleFor(x => x.Token.IsUsed)
                .NotNull()
                .Equal(false)
                .WithMessage("A Personal Access Token must not be used to be valid.");

            RuleFor(x => x.Token.ExpiresAt)
                .NotEmpty()
                .WithMessage("The ExpiresAt date is required.");

            RuleFor(x => x.Token.ExpiresAt)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("The Expiration date must be greater than the current date");

            RuleFor(x => x.Token.ExpiresAt)
                .NotEmpty()
                .GreaterThan(x => x.Token.CreatedAt)
                .WithMessage("The ExpiresAt date must be after the CreatedAt date.");

            RuleFor(x => x.Token.CreatedAt)
                .NotEmpty()
                .WithMessage("The Created At date is required.");

            RuleFor(x => x.Token.CreatedAt)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("The Creation date must be smaller or equal than the current date");
        }
    }
}
