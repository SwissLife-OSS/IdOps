using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;

namespace IdOps.GraphQL.DataLoaders
{
    public class IdentityServerGroupByIdDataLoader(
        IIdentityServerGroupService identityServerGroupService,
        IBatchScheduler batchScheduler)
        : BatchDataLoader<Guid, IdentityServerGroup>(batchScheduler, new DataLoaderOptions())
    {
        protected override async Task<IReadOnlyDictionary<Guid, IdentityServerGroup>> LoadBatchAsync(
            IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityServerGroup>? groups = await identityServerGroupService
                .GetAllGroupsAsync(cancellationToken);

            return groups.ToDictionary(x => x.Id);
        }
    }
}
