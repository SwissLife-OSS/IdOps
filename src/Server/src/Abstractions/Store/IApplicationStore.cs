using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IApplicationStore : ITenantResourceStore<Application>
    {
        Task<Application?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

        Task<SearchResult<Application>> SearchAsync(
            SearchApplicationsRequest request,
            CancellationToken cancellationToken);
    }
}
