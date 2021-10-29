using System.Threading.Tasks;
using IdOps.Security;
using Microsoft.AspNetCore.Authorization;

namespace IdOps.Authorization
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public PermissionAuthorizationHandler(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasPermissionRequirement requirement)
        {
            if (_userContextAccessor.Context is null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            IUserContext userContext = _userContextAccessor.Context;

            if (userContext.HasPermission(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
