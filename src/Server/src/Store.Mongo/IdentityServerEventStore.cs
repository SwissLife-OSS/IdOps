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
        private readonly IClientService _clientService;

        public IdentityServerEventStore(
            IIdOpsDbContext dbContext,
            IApplicationService applicationService,
            IClientService clientService)
        {
            _applicationService = applicationService;
            _clientService = clientService;
            Collection = dbContext.IdentityServerEvents;
        }

        protected IMongoCollection<IdentityServerEvent> Collection { get; }

        public async Task CreateManyAsync(
            IEnumerable<IdentityServerEvent> events,
            CancellationToken cancellationToken)
        {
            InsertManyOptions insertManyOptions = new InsertManyOptions
            {
                IsOrdered = false
            };

            await Collection
                .InsertManyAsync(events, insertManyOptions, cancellationToken);
        }

        public async Task<SearchResult<IdentityServerEvent>> SearchAsync(
            SearchIdentityServerEventsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerEvent> filter =
                Filter.Empty;

            var clientIds = new List<string>();

            if (request.Environments is { } envs && envs.Any())
            {
                filter &= Filter.In(x => x.EnvironmentName, request.Environments);
            }

            if (request.Clients is { } ids && ids.Any())
            {
                clientIds.AddRange(request.Clients);
            }

            if (request.Applications is { } apps && apps.Any())
            {
                IReadOnlyList<Application> applications = await _applicationService
                    .GetByIdsAsync(request.Applications, cancellationToken);

                IReadOnlyList<Client> clients = await _clientService
                    .GetByIdsAsync(applications.SelectMany(x => x.ClientIds), cancellationToken);

                clientIds.AddRange(clients.Select(x => x.ClientId));
            }

            if (clientIds.Count > 0)
            {
                filter &= Filter.In(x => x.ClientId, clientIds);
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
