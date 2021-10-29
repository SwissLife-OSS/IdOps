using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IClientStore : ITenantResourceStore<Client>
    {
        Task<SearchResult<Client>> SearchAsync(
            SearchClientsRequest request,
            CancellationToken cancellationToken);
    }
}
