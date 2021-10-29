using System.Threading.Tasks;

namespace IdOps.IdentityServer
{
    public interface IPersonalAccessTokenSource
    {
        string Kind { get; }

        Task<PersonalAccessTokenValidationResult> ValidateAsync(
            PersonalAccessTokenValidationContext context,
            PersonalAccessTokenMatch pat);
    }
}
