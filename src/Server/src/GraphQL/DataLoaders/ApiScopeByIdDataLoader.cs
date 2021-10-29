using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;

namespace IdOps.GraphQL.DataLoaders
{
    public class ApiScopeByIdDataLoader : BatchDataLoader<Guid, ApiScope>
    {
        private readonly IApiScopeService _apiScopeService;

        public ApiScopeByIdDataLoader(
            IApiScopeService apiScopeService,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _apiScopeService = apiScopeService;
        }

        protected override async Task<IReadOnlyDictionary<Guid, ApiScope>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<ApiScope> scopes = await _apiScopeService.GetManyAsync(
                keys,
                cancellationToken);

            return scopes.ToDictionary(x => x.Id);
        }
    }
}
