using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IApiResourceService : IResourceService<ApiResource>
    {
        Task<IReadOnlyList<ApiResource>> GetByTenantAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetDependenciesAsync(
            ApiResource apiResource,
            CancellationToken cancellationToken);

        Task<ApiResource> SaveAsync(SaveApiResourceRequest request, CancellationToken cancellationToken);
        Task<IReadOnlyList<IResource>> GetDependenciesAsync(Guid id, CancellationToken cancellationToken);
        Task<ApiResource> RemoveSecretAsync(RemoveApiSecretRequest request, CancellationToken cancellationToken);
        Task<(ApiResource, string)> AddSecretAsync(AddApiSecretRequest request, CancellationToken cancellationToken);
    }
}
