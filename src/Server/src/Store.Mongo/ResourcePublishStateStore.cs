using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static MongoDB.Driver.Builders<IdOps.Model.ResourcePublishState>;

namespace IdOps.Server.Storage.Mongo
{
    public class ResourcePublishStateStore : IResourcePublishStateStore
    {
        public ResourcePublishStateStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ResourcePublishStates;
        }

        protected IMongoCollection<ResourcePublishState> Collection { get; }

        public async Task<IEnumerable<ResourcePublishState>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourcePublishState> filter =
                FilterDefinition<ResourcePublishState>.Empty;

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ResourcePublishState>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourcePublishState> filter = Filter.In(x => x.ResourceId, resourceIds);
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<ResourcePublishState> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourcePublishState> filter = Filter.Eq(x => x.Id, id);
            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<ResourcePublishState> SaveAsync(
            ResourcePublishState publishState,
            CancellationToken cancellationToken)
        {
            //TODO: Replace with compound key
            FilterDefinition<ResourcePublishState> filter =
                Filter.Eq(x => x.ResourceId, publishState.ResourceId) &
                Filter.Eq(x => x.EnvironmentId, publishState.EnvironmentId);

            ResourcePublishState existing = await Collection
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
