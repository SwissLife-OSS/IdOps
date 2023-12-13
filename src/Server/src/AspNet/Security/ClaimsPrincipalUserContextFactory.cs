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
            if (principal is not null)
            {
                var userId = principal.FindFirstValue("sub");

                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("sub claim is missing");
                }

                var name = principal.FindFirstValue("name");

                if (string.IsNullOrEmpty(name))
                {
                    throw new InvalidOperationException("name claim is missing");
                }

                var roles = principal.Claims
                    .Where(x => x.Type == "role" && x.Value.StartsWith("IdOps"))
                    .Select(x => x.Value)
                    .ToList();

                return new User(userId, name, roles);
            }

            throw new InvalidOperationException("principal is null");
        }
    }
}
