using FluentValidation;

namespace IdOps.IdentityServer
{
    public class LocalPersonalAccessTokenAuthenticationValidator
        : PersonalAccessTokenAuthenticationValidator
    {
        public LocalPersonalAccessTokenAuthenticationValidator()
        {
            RuleFor(x => x.Definition.TokenSource)
                .NotEmpty()
                .Equal(Wellknown.PersonalAccessTokens.Source.Local)
                .WithMessage("The Source must be LOCAL.");
        }
    }
}
