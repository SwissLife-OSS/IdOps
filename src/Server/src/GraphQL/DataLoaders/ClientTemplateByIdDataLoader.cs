using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;
using IdOps.Store;

namespace IdOps.GraphQL.DataLoaders
{
    public class ClientTemplateByIdDataLoader : BatchDataLoader<Guid, ClientTemplate>
    {
        private readonly IClientTemplateStore _clientTemplateStore;

        public ClientTemplateByIdDataLoader(
            IClientTemplateStore clientTemplateStore,
            IBatchScheduler batchScheduler) : base(batchScheduler)
        {
            _clientTemplateStore = clientTemplateStore;
        }

        protected override async Task<IReadOnlyDictionary<Guid, ClientTemplate>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<ClientTemplate> templates = await _clientTemplateStore.GetManyAsync(
                keys,
                cancellationToken);

            return templates.ToDictionary(x => x.Id);

        }
    }
}
