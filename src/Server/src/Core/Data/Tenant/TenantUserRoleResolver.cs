using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Server.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace IdOps
{
    public class TenantUserRoleResolver : ITenantUserRoleResolver
    {
        private readonly ITenantStore _tenantStore;
        private readonly IMemoryCache _memoryCache;

        public TenantUserRoleResolver(
            ITenantStore tenantStore,
            IMemoryCache memoryCache)
        {
            _tenantStore = tenantStore;
            _memoryCache = memoryCache;
        }

        public async Task<IReadOnlyDictionary<string, IList<TenantRole>>>
            GetClaimRoleMappingsAsync(CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync("_tenant.rolemap", async (entry) =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.AddHours(1);

                return await GetClaimRoleMappingsInternalAsync(cancellationToken);
            });
        }


        private async Task<IReadOnlyDictionary<string, IList<TenantRole>>>
            GetClaimRoleMappingsInternalAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Tenant> allTenants = await _tenantStore.GetAllAsync(cancellationToken);

            Dictionary<string, IList<TenantRole>> mapping = new();

            foreach (Tenant? tenant in allTenants)
            {
                foreach (TenantRoleMapping? map in tenant.RoleMappings)
                {
                    var item = new TenantRole(tenant.Id, map.Role)
                    {
                        EnvironmentId = map.EnvironmentId
                    };
                    if (mapping.ContainsKey(map.ClaimValue))
                    {
                        mapping[map.ClaimValue].Add(item);
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
}
