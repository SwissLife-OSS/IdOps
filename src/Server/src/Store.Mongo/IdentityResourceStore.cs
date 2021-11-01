using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.IdentityResource>;

namespace IdOps.Server.Storage.Mongo
{
    public class IdentityResourceStore : ResourceStore<IdentityResource>, IIdentityResourceStore
    {
        public IdentityResourceStore(IIdOpsDbContext dbContext) : base(dbContext.IdentityResources)
        {
        }

        public override async Task<IReadOnlyList<IdentityResource>> GetResourceWithOpenApproval(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityResource> filter = Filter.Empty;

            if (tenants != null)
            {
                filter &= Filter.ElemMatch(x => x.Tenants, x => tenants.Contains(x));
            }

            if (ids != null)
            {
                filter &= Filter.In(x => x.Id, ids);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public override async Task<IReadOnlyList<IdentityResource>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityResource> filter = FilterDefinition<IdentityResource>.Empty;

            if (tenants != null)
            {
                filter &= Filter.ElemMatch(x => x.Tenants, x => tenants.Contains(x));
            }

            if (ids != null)
            {
                filter &= Filter.In(x => x.Id, ids);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
