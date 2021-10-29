using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public class InMemoryPublishingContextFactory : IPublishingContextFactory
    {
        private readonly IResourceStores _stores;

        public InMemoryPublishingContextFactory(IResourceStores stores)
        {
            _stores = stores;
        }

        public async ValueTask<IPublishingContext> CreateAsync(
            Guid environmentId,
            CancellationToken cancellationToken)
        {
            return new InMemoryPublishingContext(
                _stores,
                await _stores.ApiResources.GetAllAsync(cancellationToken),
                await _stores.IdentityResources.GetAllAsync(cancellationToken),
                await _stores.ApiScopes.GetAllAsync(cancellationToken),
                await _stores.Environments.GetAllAsync(cancellationToken),
                environmentId);
        }
    }
}
