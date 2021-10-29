using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public interface IResourceMessageFactory<T> : IResourceMessageFactory
        where T : class, IResource, new()
    {
        ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            T resource,
            CancellationToken cancellationToken);

        ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            T resource,
            CancellationToken cancellationToken);
    }

    public interface IResourceMessageFactory
    {
        string ResourceType { get; }

        ValueTask<IdOpsResource?> BuildPublishMessageAsync(
            IPublishingContext context,
            IResource resource,
            CancellationToken cancellationToken);

        ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            IResource resource,
            CancellationToken cancellationToken);
    }
}
