using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;

namespace IdOps
{
    public class TenantService : UserTenantService, ITenantService
    {
        private readonly ITenantStore _tenantStore;

        public TenantService(
            ITenantStore tenantStore,
            IUserContextAccessor userContextAccessor)
                : base(userContextAccessor)
        {
            _tenantStore = tenantStore;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants = await GetUserTenantsAsync(cancellationToken);

            return await _tenantStore.GetManyAsync(userTenants, cancellationToken);
        }

        public async Task<Tenant> SaveAsync(
            SaveTenantRequest request,
            CancellationToken cancellationToken)
        {
            var tenant = new Tenant
            {
                Id = request.Id,
                Color = request.Color,
                Description = request.Description,
                Modules = request.Modules?.ToList() ?? new List<TenantModule>(),
                RoleMappings = request.RoleMappings?.ToList() ?? new List<TenantRoleMapping>(),
                Emails = request.Emails?.ToList() ?? new List<string>()
            };

            return await _tenantStore.SaveAsync(tenant, cancellationToken);
        }
    }
}
