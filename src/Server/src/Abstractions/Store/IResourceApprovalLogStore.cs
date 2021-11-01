using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IResourceApprovalLogStore
    {
        Task CreateAsync(ResourceApprovalLog log, CancellationToken cancellationToken);

        Task<IEnumerable<ResourceApprovalLog>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken);

        Task<SearchResult<ResourceApprovalLog>> SearchAsync(
            SearchResourceApprovalLogsRequest request,
            CancellationToken cancellationToken);
    }
}
