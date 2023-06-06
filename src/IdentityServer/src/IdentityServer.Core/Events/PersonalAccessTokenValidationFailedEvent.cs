using System;
using System.Collections.Generic;
using System.Linq;
using Duende.IdentityServer.Events;
using FluentValidation.Results;

namespace IdOps.IdentityServer.Events
{
    public class PersonalAccessTokenValidationFailedEvent : Event
    {
        public PersonalAccessTokenValidationFailedEvent(
            string clientId,
            string? userName,
            string? errorMessage,
            string? scopes,
            IList<ValidationFailure>? errors)
            : base(
                "PersonalAccessToken",
                "PAT validation failed",
                EventTypes.Failure,
                IdOpsEventIds.PersonalAccessTokenValidationFailed,
                errorMessage)
        {
            ClientId = clientId;
            UserName = userName;
            Scopes = scopes;
            Errors = errors;
        }

        public string ClientId { get; }

        public string? Scopes { get; }

        public string? UserName { get; }

        public IList<ValidationFailure>? Errors { get; }

        public static PersonalAccessTokenValidationFailedEvent New(
            string clientId,
            string? userName,
            string? errorMessage,
            IEnumerable<string>? scopes,
            IList<ValidationFailure>? errors) =>
            new(clientId,
                userName,
                errorMessage,
                string.Join(",", scopes ?? Enumerable.Empty<string>()),
                errors);
    }
}
