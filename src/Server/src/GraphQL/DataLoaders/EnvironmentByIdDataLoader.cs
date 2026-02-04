using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace IdOps.GraphQL.DataLoaders
{
    public class EnvironmentByIdDataLoader(
        IEnvironmentService environmentService,
        IBatchScheduler batchScheduler)
        : BatchDataLoader<Guid, Model.Environment>(batchScheduler, new DataLoaderOptions())
    {
        protected override async Task<IReadOnlyDictionary<Guid, Model.Environment>> LoadBatchAsync(
            IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<Model.Environment> environments = await environmentService
                .GetAllAsync(cancellationToken);

            return environments
                .Where(x => keys.Contains(x.Id))
                .ToDictionary(x => x.Id);
        }
    }
}
