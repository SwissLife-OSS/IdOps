using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IResourcePublishLogStore
    {
        Task CreateAsync(ResourcePublishLog log, CancellationToken cancellationToken);
        Task<IEnumerable<ResourcePublishLog>> GetManyAsync(IEnumerable<Guid> resourceIds, CancellationToken cancellationToken);
        Task<SearchResult<ResourcePublishLog>> SearchAsync(SearchResourcePublishLogsRequest request, CancellationToken cancellationToken);
    }
}
