using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;

namespace IdOps
{
    public abstract class ResourceService<T> : UserTenantService, IResourceService<T>
        where T : class, IResource, new()
    {
        private static string _resourceName = typeof(T).Name;

        public ResourceService(IUserContextAccessor userContextAccessor)
            : base(userContextAccessor)
        {
        }

        public bool IsOfType(IResource resource) => resource is T;

        public bool IsOfType(string resource) => resource == _resourceName;

        public string ResourceType => _resourceName;

        async ValueTask<IResource?> IResourceService.GetResourceByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            await GetResourceByIdAsync(id, cancellationToken);

        public abstract ValueTask<T?> GetResourceByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

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
