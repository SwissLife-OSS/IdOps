using System.Threading.Tasks;

namespace IdOps.IdentityServer
{
    public interface IPersonalAccessTokenValidator
    {
        Task<PersonalAccessTokenValidationResult> ValidateAsync(
            PersonalAccessTokenValidationContext context);
    }
}
