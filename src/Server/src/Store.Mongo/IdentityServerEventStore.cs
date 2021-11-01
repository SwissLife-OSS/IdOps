using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.IdentityServerEvent>;

namespace IdOps.Server.Storage.Mongo
{
    public class IdentityServerEventStore : IIdentityServerEventStore
    {
        public IdentityServerEventStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.IdentityServerEvents;
        }

        protected IMongoCollection<IdentityServerEvent> Collection { get; }

        public async Task CreateManyAsync(
            IEnumerable<IdentityServerEvent> events,
            CancellationToken cancellationToken)
        {
            await Collection
                .InsertManyAsync(events, options: null, cancellationToken);
        }

        public async Task<SearchResult<IdentityServerEvent>> SearchAsync(
            SearchIdentityServerEventsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerEvent> filter =
                Filter.Empty;

            if (request.Environment is { } env && env != "ALL")
            {
                filter &= Filter.Eq(x => x.EnvironmentName, request.Environment);
            }

            if (!string.IsNullOrEmpty(request.ClientId))
            {
                filter &= Filter.Eq(x => x.ClientId, request.ClientId);
            }

            if (request.EventTypes is { } t && t.Any())
            {
                filter &= Filter.In(x => x.EventType, t);
            }

            SortDefinition<IdentityServerEvent> sort = Sort.Descending(x => x.TimeStamp);

            return await Collection.ExecuteSearchAsync(filter, request, sort, cancellationToken);
        }
    }
}
