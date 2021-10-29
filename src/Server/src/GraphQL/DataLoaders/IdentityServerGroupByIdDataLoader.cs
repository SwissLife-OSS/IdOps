using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using IdOps.Model;

namespace IdOps.GraphQL.DataLoaders
{
    public class IdentityServerGroupByIdDataLoader : BatchDataLoader<Guid, IdentityServerGroup>
    {
        private readonly IIdentityServerService _identityServerService;

        public IdentityServerGroupByIdDataLoader(
            IIdentityServerService identityServerService,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _identityServerService = identityServerService;
        }

        protected override async Task<IReadOnlyDictionary<Guid, IdentityServerGroup>> LoadBatchAsync(
            IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityServerGroup>? groups = await _identityServerService
                .GetAllGroupsAsync(cancellationToken);

            return groups.ToDictionary(x => x.Id);
        }
    }
}
