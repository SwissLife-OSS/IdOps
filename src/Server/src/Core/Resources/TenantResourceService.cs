using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Configuration;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public abstract class TenantResourceService<T> : ResourceService<T>
        where T : class, ITenantResource, new()
    {
        private readonly ITenantResourceStore<T> _store;

        protected TenantResourceService(
            IdOpsServerOptions options,
            IUserContextAccessor accessor,
            ITenantResourceStore<T> store)
            : base(options, accessor, store)
        {
            _store = store;
        }

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
