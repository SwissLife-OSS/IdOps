using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public abstract class ResourceMessageFactory<T>
        : IResourceMessageFactory<T>
        where T : class, IResource, new()
    {
        private static readonly string _resourceType = typeof(T).Name;

        public string ResourceType => _resourceType;

        public abstract ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            T resource,
            CancellationToken cancellationToken);

        public abstract ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            T resource,
            CancellationToken cancellationToken);

        public async ValueTask<IdOpsResource?> BuildPublishMessageAsync(
            IPublishingContext context,
            IResource resource,
            CancellationToken cancellationToken)
        {
            if (resource is T resourceOfT)
            {
                return await BuildPublishMessage(context, resourceOfT, cancellationToken);
            }

            return null;
        }

        public async ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            IResource resource,
            CancellationToken cancellationToken)
        {
            if (resource is T resourceOfT)
            {
                return await ResolveServerGroupAsync(resourceOfT, cancellationToken);
            }

            return null;
        }
    }
}
