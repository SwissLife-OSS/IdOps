using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;
using IdOps.Store;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class InsightsQueries
    {
        public async Task<SearchResult<IdentityServerEvent>> SearchIdentityServerEvents(
            [Service] IIdentityServerEventStore eventStore,
            SearchIdentityServerEventsRequest input,
            CancellationToken cancellationToken)
        {
            return await eventStore.SearchAsync(input, cancellationToken);
        }
    }
}
