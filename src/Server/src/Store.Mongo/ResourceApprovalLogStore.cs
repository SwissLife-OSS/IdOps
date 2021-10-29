using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.ResourceApprovalLog>;

namespace IdOps.Store.Mongo
{
    public class ResourceApprovalLogStore : IResourceApprovalLogStore
    {
        public ResourceApprovalLogStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ResourceApprovalLogs;
        }

        protected IMongoCollection<ResourceApprovalLog> Collection { get; }

        public async Task<IEnumerable<ResourceApprovalLog>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceApprovalLog> filter = Filter.In(x => x.ResourceId, resourceIds);
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(
            ResourceApprovalLog log,
            CancellationToken cancellationToken)
        {
            await Collection.InsertOneAsync(log, options: null, cancellationToken);
        }

        public async Task<SearchResult<ResourceApprovalLog>> SearchAsync(
            SearchResourceApprovalLogsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceApprovalLog> filter = Filter.Empty;

            if (request.EnvironmentId is { })
            {
                filter &= Filter.Eq(x => x.EnvironmentId, request.EnvironmentId);
            }

            if (request.ResourceId is { })
            {
                filter &= Filter.Eq(x => x.ResourceId, request.ResourceId);
            }

            return await Collection.ExecuteSearchAsync(filter, request, cancellationToken);
        }
    }
}
