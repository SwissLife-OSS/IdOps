using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IPublishingService
    {
        IAsyncEnumerable<PublishedResource> GetPublishedResourcesAsync(
            PublishedResourcesRequest? filter,
            CancellationToken cancellationToken);
    }

    public record PublishResourceRequest(
        IReadOnlyList<Guid> Resources,
        Guid DestinationEnvionmentId);
}
