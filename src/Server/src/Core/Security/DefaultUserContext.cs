using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Security
{
    public class DefaultUserContext : IUserContext
    {
        private readonly User _user;
        private readonly ITenantUserRoleResolver _tenantUserRoleResolver;
        private HashSet<string> _permissions;

        public bool IsAuthenticated => true;

        public string UserId => _user.Id;

        public User User => _user;

        public DefaultUserContext(User user, ITenantUserRoleResolver tenantUserRoleResolver)
        {
            _user = user;
            _tenantUserRoleResolver = tenantUserRoleResolver;
            _permissions = GetPermissions(user);
        }

        private HashSet<string> GetPermissions(User user)
        {
            var permissions = new HashSet<string>();

            if (user.Roles is { } roles && roles.Any())
            {
                foreach (var role in roles)
                {
                    if (Security.Permissions.RoleMap.ContainsKey(role))
                    {
                        permissions.UnionWith(Security.Permissions.RoleMap[role]);
                    }
                }
            }

            return permissions;
        }

        public IEnumerable<string> Roles => _user.Roles;

        public IEnumerable<string> Permissions => _permissions;

        public async Task<IReadOnlyList<string>> GetTenantsAsync(
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<string, IList<TenantRole>> maps = await _tenantUserRoleResolver
                .GetClaimRoleMappingsAsync(cancellationToken);

            var userTenants = new List<string>();

            foreach ( var role in Roles)
            {
                if (maps.ContainsKey(role))
                {
                    userTenants.AddRange(maps[role].Select(x => x.TenantId));
                }
            }

            return userTenants;
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role, StringComparer.InvariantCulture);
        }

        public bool HasPermission(string permission)
        {
            return _permissions.Contains(permission, StringComparer.InvariantCulture);
        }
    }
}
