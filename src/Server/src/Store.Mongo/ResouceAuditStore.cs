using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.ResourceAuditEvent>;

namespace IdOps.Store.Mongo
{
    public class ResouceAuditStore : IResouceAuditStore
    {
        public ResouceAuditStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ResouceAudits;
        }

        protected IMongoCollection<ResourceAuditEvent> Collection { get; }

        public async Task CreateAsync(
            ResourceAuditEvent audit,
            CancellationToken cancellationToken)
        {
            await Collection.InsertOneAsync(audit, options: null, cancellationToken);
        }

        public async Task<SearchResult<ResourceAuditEvent>> SearchAsync(
            SearchResourceAuditRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ResourceAuditEvent> filter = Filter.Empty;

            if (request.ResourceId.HasValue)
            {
                filter &= Filter.Eq(x => x.ResourceId, request.ResourceId.Value);
            }

            if (request.UserId is { } userId)
            {
                filter &= Filter.Eq(x => x.UserId, userId);
            }

            return await Collection.ExecuteSearchAsync(filter, request, cancellationToken);
        }
    }
}
