using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace IdOps.GraphQL.DataLoaders
{
    public class EnvironmentByIdDataLoader : BatchDataLoader<Guid, Model.Environment>
    {
        private readonly IEnvironmentService _environmentService;

        public EnvironmentByIdDataLoader(
            IEnvironmentService environmentService,
            IBatchScheduler batchScheduler)
                : base(batchScheduler)
        {
            _environmentService = environmentService;
        }

        protected override async Task<IReadOnlyDictionary<Guid, Model.Environment>> LoadBatchAsync(
            IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<Model.Environment> environments = await _environmentService
                .GetAllAsync(cancellationToken);

            return environments
                .Where(x => keys.Contains(x.Id))
                .ToDictionary(x => x.Id);
        }
    }
}
