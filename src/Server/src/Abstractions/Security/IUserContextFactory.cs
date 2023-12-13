using System.Security.Claims;

namespace IdOps.Security;

public interface IUserContextFactory
{
    IUserContext Create();

    IUserContext Create(ClaimsPrincipal? principal);
}
