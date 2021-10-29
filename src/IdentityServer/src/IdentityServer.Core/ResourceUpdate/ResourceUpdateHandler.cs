using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Messages;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class ResourceUpdateHandler : IResourceUpdateHandler
    {
        private readonly IResourceConsumer _resourceConsumer;

        public ResourceUpdateHandler(IResourceConsumer resourceConsumer)
        {
            _resourceConsumer = resourceConsumer;
        }

        public async Task<UpdateResourceResult> HandleUpdateAsync(
            PublishResourceMessage update,
            CancellationToken cancellationToken) =>
            await _resourceConsumer
                .ProcessAsync(update.ResourceType, update.Data, cancellationToken);
    }
}
