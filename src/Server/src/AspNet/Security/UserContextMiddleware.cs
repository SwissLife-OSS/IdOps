using System.Threading.Tasks;
using IdOps.Security;
using Microsoft.AspNetCore.Http;

namespace IdOps.Api.Security
{
    public class UserContextMiddleware
    {
        private readonly IUserContextFactory _userContextFactory;
        private readonly IUserContextAccessor _userContextAccessor;
        private readonly RequestDelegate _next;

        public UserContextMiddleware(
            RequestDelegate next,
            IUserContextFactory userContextFactory,
            IUserContextAccessor userContextAccessor)
        {
            _next = next;
            _userContextFactory = userContextFactory;
            _userContextAccessor = userContextAccessor;
        }

        public Task Invoke(HttpContext context)
        {
            _userContextAccessor.Context = _userContextFactory.Create();

            return _next(context);
        }
    }
}
