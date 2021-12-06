using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IPublishingService
    {
        IAsyncEnumerable<PublishedResource> GetPublishedResourcesAsync(
            PublishedResourcesRequest? filter,
            CancellationToken cancellationToken);

        Task<IEnumerable<ResourcePublishLog>> GetResourcePublishingLog(
            IReadOnlyList<Guid> resourceIds,
            CancellationToken cancellationToken);
    }

    public record PublishResourceRequest(
        IReadOnlyList<Guid> Resources,
        Guid DestinationEnvionmentId);
}
