using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Security;

public sealed class DefaultUserContext : IUserContext
{
    private readonly ITenantUserRoleResolver _tenantUserRoleResolver;

    private readonly HashSet<string> _permissions;

    public DefaultUserContext(User user, ITenantUserRoleResolver tenantUserRoleResolver)
    {
        User = user;
        _tenantUserRoleResolver = tenantUserRoleResolver;
        _permissions = user.GetPermissions();
    }

    public bool IsAuthenticated => true;

    public User User { get; }

    public string UserId => User.Id;

    public IEnumerable<string> Roles => User.Roles;

    public IEnumerable<string> Permissions => _permissions;

    public async Task<IReadOnlyList<string>> GetTenantsAsync(CancellationToken ct)
    {
        var roleMap = await _tenantUserRoleResolver.GetClaimRoleMappingsAsync(ct);

        var userTenants = new List<string>();

        foreach (var role in Roles)
        {
            if (roleMap.ContainsKey(role))
            {
                userTenants.AddRange(roleMap[role].Select(x => x.TenantId));
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
        return _permissions.Contains(permission);
    }
}

static file class Extensions
{
    public static HashSet<string> GetPermissions(this User user)
    {
        var permissions = new HashSet<string>();

        if (user.Roles is { Count: > 0 } roles)
        {
            foreach (var role in roles)
            {
                if (Permissions.RoleMap.TryGetValue(role, out List<string>? value))
                {
                    permissions.UnionWith(value);
                }
            }
        }

        return permissions;
    }
}
