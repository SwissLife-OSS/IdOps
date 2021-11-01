using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;
using IdOps.Server.Storage;

namespace IdOps.GraphQL.DataLoaders
{
    public class TenantByIdDataLoader : BatchDataLoader<string, Tenant>
    {
        private readonly ITenantStore _tenantStore;

        public TenantByIdDataLoader(
            ITenantStore tenantStore,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _tenantStore = tenantStore;
        }

        protected override async Task<IReadOnlyDictionary<string, Tenant>> LoadBatchAsync(
            IReadOnlyList<string> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<Tenant>? tenants = await _tenantStore
                .GetManyAsync(keys, cancellationToken);

            return tenants.ToDictionary(x => x.Id);
        }
    }
}
