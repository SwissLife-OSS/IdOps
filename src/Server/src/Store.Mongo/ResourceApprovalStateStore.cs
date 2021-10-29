using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.ResourceApprovalState>;

namespace IdOps.Store.Mongo
{
    public class ResourceApprovalStateStore : IResourceApprovalStateStore
    {
        public ResourceApprovalStateStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ResourceApprovalStates;
        }

        protected IMongoCollection<ResourceApprovalState> Collection { get; }

        public async Task<IReadOnlyList<ResourceApprovalState>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceApprovalState> filter =
                FilterDefinition<ResourceApprovalState>.Empty;

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ResourceApprovalState>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceApprovalState> filter =
                Filter.In(x => x.ResourceId, resourceIds);
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<ResourceApprovalState> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceApprovalState> filter = Filter.Eq(x => x.Id, id);
            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<ResourceApprovalState> SaveAsync(
            ResourceApprovalState publishState,
            CancellationToken cancellationToken)
        {
            //TODO: Replace with compound key
            FilterDefinition<ResourceApprovalState> filter =
                Filter.Eq(x => x.ResourceId, publishState.ResourceId) &
                Filter.Eq(x => x.EnvironmentId, publishState.EnvironmentId);

            ResourceApprovalState existing = await Collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);

            publishState.Id = existing switch
            {
                { } e => e.Id,
                _ => Guid.NewGuid()
            };

            await Collection.ReplaceOneAsync(
                x => x.Id == publishState.Id,
                publishState,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            return publishState;
        }
    }
}
