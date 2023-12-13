using System.Security.Claims;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Security;

public sealed class HttpContextEnricher : IRequestContextEnricher
{
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUserContextFactory _userContextFactory;

    public HttpContextEnricher(
        IUserContextAccessor userContextAccessor,
        IUserContextFactory userContextFactory)
    {
        _userContextAccessor = userContextAccessor;
        _userContextFactory = userContextFactory;
    }

    /// <inheritdoc />
    public void Enrich(IRequestContext context)
    {
        if (context.ContextData.TryGetValue(nameof(ClaimsPrincipal), out var value) &&
            value is ClaimsPrincipal principal)
        {
            _userContextAccessor.Context = _userContextFactory.Create(principal);
        }
        else
        {
            context.ContextData[WellKnownContextData.UserState] =
                new UserState(new ClaimsPrincipal(), false);
        }
    }
}
