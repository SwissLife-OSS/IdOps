using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;

namespace IdOps.GraphQL.DataLoaders
{
    public class ApiScopeByIdDataLoader(
        IApiScopeService apiScopeService,
        IBatchScheduler batchScheduler)
        : BatchDataLoader<Guid, ApiScope>(batchScheduler, new DataLoaderOptions())
    {
        protected override async Task<IReadOnlyDictionary<Guid, ApiScope>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<ApiScope> scopes = await apiScopeService.GetManyAsync(
                keys,
                cancellationToken);

            return scopes.ToDictionary(x => x.Id);
        }
    }
}
