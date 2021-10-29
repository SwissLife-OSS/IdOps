using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Security;
using IdOps.Store;

namespace IdOps
{
    public abstract class TenantResourceService<T> : ResourceService<T>, IResourceService<T>
        where T : class, ITenantResource, new()
    {
        private readonly ITenantResourceStore<T> _store;

        protected TenantResourceService(
            IUserContextAccessor accessor,
            ITenantResourceStore<T> store)
            : base(accessor)
        {
            _store = store;
        }

        public override async ValueTask<T?> GetResourceByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            await _store.GetByIdAsync(id, cancellationToken);

        public override async Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants =
                await GetUserMergedTenantsAsync(
                    tenants?.ToArray() ?? Array.Empty<string>(),
                    cancellationToken);

            return await _store.GetByTenantsAsync(ids, userTenants, cancellationToken);
        }
    }
}
