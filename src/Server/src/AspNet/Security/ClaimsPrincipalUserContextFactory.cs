using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdOps.Security;
using Microsoft.AspNetCore.Http;

namespace IdOps.Api.Security
{
    public class ClaimsPrincipalUserContextFactory : IUserContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantUserRoleResolver _tenantUserRoleResolver;

        public ClaimsPrincipalUserContextFactory(
            IHttpContextAccessor httpContextAccessor,
            ITenantUserRoleResolver tenantUserRoleResolver)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantUserRoleResolver = tenantUserRoleResolver;
        }

        public IUserContext Create()
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

            return Create(principal);
        }

        public IUserContext Create(ClaimsPrincipal? principal)
        {
            User user = CreateUser(principal);

            return new DefaultUserContext(user, _tenantUserRoleResolver);
        }

        private User CreateUser(ClaimsPrincipal? principal)
        {
            if (principal is { })
            {
                string userId = principal.FindFirstValue("sub");
                string name = principal.FindFirstValue("name");

                IEnumerable<string> roles = principal.Claims
                    .Where(x => x.Type == "role" && x.Value.StartsWith("IdOps"))
                    .Select(x => x.Value);

                return new User(userId, name, roles.ToList());
            }

            throw new InvalidOperationException("principal is null");
        }
    }
}
