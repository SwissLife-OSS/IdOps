using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Security;

namespace IdOps
{
    public class UserTenantService
    {
        private readonly IUserContextAccessor _userContextAccessor;

        protected UserTenantService(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        protected IUserContext UserContext
        {
            get
            {
                return _userContextAccessor.Context ??
                    throw CouldNotAccessUserContextException.New();
            }
        }

        protected async Task<IReadOnlyList<string>> GetUserTenantsAsync(
            CancellationToken cancellationToken)
        {
            return await UserContext.GetTenantsAsync(cancellationToken);
        }

        protected async Task<IReadOnlyList<string>> GetUserMergedTenantsAsync(
            IReadOnlyList<string>? tenants,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants = await GetUserTenantsAsync(cancellationToken);

            tenants ??= Array.Empty<string>();

            if (tenants.Count > 0)
            {
                tenants = userTenants.Intersect(tenants).ToArray();
            }
            else
            {
                tenants = userTenants;
            }

            return tenants;
        }

        protected async Task ValidateTenantAccess(
            ITenantResource token,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants =
                await UserContext.GetTenantsAsync(cancellationToken);

            if (!userTenants.Contains(token.Tenant))
            {
                throw new Exception("You are not authorized to do these changes");
            }
        }
    }
}
