using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;
using IdOps.Server.Storage;

namespace IdOps.GraphQL.DataLoaders
{
    public class ClientTemplateByIdDataLoader(
        IClientTemplateStore clientTemplateStore,
        IBatchScheduler batchScheduler)
        : BatchDataLoader<Guid, ClientTemplate>(batchScheduler, new DataLoaderOptions())
    {
        protected override async Task<IReadOnlyDictionary<Guid, ClientTemplate>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<ClientTemplate> templates = await clientTemplateStore.GetManyAsync(
                keys,
                cancellationToken);

            return templates.ToDictionary(x => x.Id);

        }
    }
}
