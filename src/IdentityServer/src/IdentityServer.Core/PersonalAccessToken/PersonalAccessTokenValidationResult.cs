using System;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace IdOps.IdentityServer
{
    public class PersonalAccessTokenValidationResult
    {
        private PersonalAccessTokenValidationResult(bool isValid)
        {
            IsValid = isValid;
            GrantValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
        }

        public PersonalAccessTokenValidationResult(GrantValidationResult grantValidationResult)
        {
            IsValid = true;
            GrantValidationResult = grantValidationResult ??
                throw new ArgumentNullException(nameof(grantValidationResult));
        }

        public bool IsValid { get; }

        public GrantValidationResult GrantValidationResult { get; }

        public static readonly PersonalAccessTokenValidationResult Invalid = new(false);
    }
}
