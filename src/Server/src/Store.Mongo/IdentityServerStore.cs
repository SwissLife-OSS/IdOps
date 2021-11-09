using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.IdentityServer>;

namespace IdOps.Server.Storage.Mongo
{
    public class IdentityServerStore : ResourceStore<Model.IdentityServer>, IIdentityServerStore
    {
        public IdentityServerStore(IIdOpsDbContext dbContext)
            : base(dbContext.IdentityServers)
        {
        }

        public override async Task<IReadOnlyList<Model.IdentityServer>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Model.IdentityServer> filter =
                FilterDefinition<Model.IdentityServer>.Empty;

            if (ids != null)
            {
                filter &= Filter.In(x => x.Id, ids);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
