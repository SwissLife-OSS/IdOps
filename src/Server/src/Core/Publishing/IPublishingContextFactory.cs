using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IPublishingContextFactory
    {
        ValueTask<IPublishingContext> CreateAsync(
            Guid environmentId,
            CancellationToken cancellationToken);
    }
}
