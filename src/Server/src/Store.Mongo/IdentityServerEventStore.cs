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
        private readonly IApplicationService _applicationService;

        public IdentityServerEventStore(IIdOpsDbContext dbContext, IApplicationService applicationService)
        {
            _applicationService = applicationService;
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

            var clients = new List<string>();

            if (request.Environments is { } envs && envs.Any())
            {
                filter &= Filter.In(x => x.EnvironmentName, request.Environments);
            }

            if (request.Clients is { } ids && ids.Any())
            {
                clients.AddRange(request.Clients);
            }

            if (request.Applications is { } apps && apps.Any())
            {
                IReadOnlyList<Application> applications = await _applicationService
                    .GetByIdsAsync(request.Applications, cancellationToken);

                clients.AddRange(applications.SelectMany(x => x.ClientIds.Select(c => c.ToString("N"))));
            }

            if (clients.Count > 0)
            {
                filter &= Filter.In(x => x.ClientId, clients);
            }

            if (request.EventTypes is { } events && events.Any())
            {
                filter &= Filter.In(x => x.EventType, request.EventTypes);
            }

            SortDefinition<IdentityServerEvent> sort = Sort.Descending(x => x.TimeStamp);

            return await Collection.ExecuteSearchAsync(filter, request, sort, cancellationToken);
        }
    }
}
