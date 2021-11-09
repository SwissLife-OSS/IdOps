using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Configuration;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public abstract class ResourceService<T> : UserTenantService, IResourceService<T>
        where T : class, IResource, new()
    {
        private readonly IdOpsServerOptions _options;
        private readonly IResourceStore<T> _store;

        protected ResourceService(
            IdOpsServerOptions options,
            IUserContextAccessor userContextAccessor,
            IResourceStore<T> store)
            : base(userContextAccessor)
        {
            _options = options;
            _store = store;
        }

        public bool RequiresApproval(Guid id)
        {
            // TODO: Refactor with Tenant modules
            return _options.NeedsApproval.Contains(ResourceType);
        }

        public string ResourceType { get; } = typeof(T).Name;

        public abstract bool IsAllowedToPublish();

        public abstract bool IsAllowedToApprove();

        async ValueTask<IResource?> IResourceService.GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            await GetByIdAsync(id, cancellationToken);

        public Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            _store.GetByIdAsync(id, cancellationToken);

        public Task<IReadOnlyList<T>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken) =>
            _store.GetByIdsAsync(ids, cancellationToken);

        public abstract Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        async Task<IReadOnlyList<IResource>> IResourceService.GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken) =>
            await GetByTenantsAsync(ids, tenants, cancellationToken);
    }
}
