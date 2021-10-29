using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IApiScopeService : IResourceService<ApiScope>
    {
        Task<IReadOnlyList<IResource>> GetDependenciesAsync(ApiScope client, CancellationToken cancellationToken);

        Task<IReadOnlyList<ApiScope>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

        Task<ApiScope> SaveAsync(SaveApiScopeRequest request, CancellationToken cancellationToken);
    }
}
