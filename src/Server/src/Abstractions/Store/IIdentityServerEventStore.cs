using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IIdentityServerEventStore
    {
        Task CreateManyAsync(
            IEnumerable<IdentityServerEvent> events,
            CancellationToken cancellationToken);

        Task<SearchResult<IdentityServerEvent>> SearchAsync(
            SearchIdentityServerEventsRequest request,
            CancellationToken cancellationToken);
    }
}
