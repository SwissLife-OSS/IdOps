using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static MongoDB.Driver.Builders<IdOps.Model.ResourcePublishLog>;

namespace IdOps.Server.Storage.Mongo
{
    public class ResourcePublishLogStore : IResourcePublishLogStore
    {
        private readonly IIdOpsDbContext _dbContext;

        public ResourcePublishLogStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ResourcePublishLogs;
        }

        protected IMongoCollection<ResourcePublishLog> Collection { get; }

        public async Task<IEnumerable<ResourcePublishLog>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourcePublishLog> filter = Filter.In(x => x.Id, resourceIds);
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(
            ResourcePublishLog log,
            CancellationToken cancellationToken)
        {
            await _dbContext.ResourcePublishLogs
                .InsertOneAsync(log, options: null, cancellationToken);
        }

        public async Task<SearchResult<ResourcePublishLog>> SearchAsync(
            SearchResourcePublishLogsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourcePublishLog> filter = Filter.Empty;

            if (request.EnvironmentId is { })
            {
                filter &= Filter.Eq(x => x.EnvironmentId, request.EnvironmentId);
            }

            if (request.ResourceId is { })
            {
                filter &= Filter.Eq(x => x.EnvironmentId, request.ResourceId);
            }

            return await _dbContext.ResourcePublishLogs
                .ExecuteSearchAsync(filter, request, cancellationToken);
        }
    }
}
