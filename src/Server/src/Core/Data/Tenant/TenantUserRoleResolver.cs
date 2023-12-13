using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Server.Storage;
using Microsoft.Extensions.Caching.Memory;
using RoleMap =
    System.Collections.Generic.IReadOnlyDictionary<string,
        System.Collections.Generic.IList<IdOps.TenantRole>>;

namespace IdOps;

public class TenantUserRoleResolver : ITenantUserRoleResolver
{
    private readonly ITenantStore _tenantStore;
    private readonly IMemoryCache _memoryCache;

    public TenantUserRoleResolver(ITenantStore tenantStore, IMemoryCache memoryCache)
    {
        _tenantStore = tenantStore;
        _memoryCache = memoryCache;
    }

    public async Task<RoleMap> GetClaimRoleMappingsAsync(CancellationToken ct)
    {
        var result = await _memoryCache.GetOrCreateAsync("_tenant.rolemap", GetOrCreate);

        return result!;

        Task<RoleMap> GetOrCreate(ICacheEntry entry)
        {
            entry.AbsoluteExpiration = DateTime.UtcNow.AddHours(1);

            return GetClaimRoleMappingsInternalAsync(ct);
        }
    }

    private async Task<RoleMap> GetClaimRoleMappingsInternalAsync(CancellationToken ct)
    {
        var allTenants = await _tenantStore.GetAllAsync(ct);

        var mapping = new Dictionary<string, IList<TenantRole>>();

        foreach (Tenant? tenant in allTenants)
        {
            foreach (TenantRoleMapping? map in tenant.RoleMappings)
            {
                var item = new TenantRole(tenant.Id, map.Role)
                {
                    EnvironmentId = map.EnvironmentId
                };

                if (mapping.TryGetValue(map.ClaimValue, out IList<TenantRole>? value))
                {
                    value.Add(item);
                }
                else
                {
                    mapping.Add(map.ClaimValue, new List<TenantRole> { item });
                }
            }
        }

        return mapping;
    }
}
