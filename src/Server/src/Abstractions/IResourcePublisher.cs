using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IResourcePublisher
    {
        Task<PublishResourcesResult> PublishResourcesAsync(
            PublishResourceRequest request,
            CancellationToken cancellationToken);
    }

    public record PublishResourcesResult(Guid JobId, IEnumerable<Guid> Resources);
}
