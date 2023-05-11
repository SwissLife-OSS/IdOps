using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IApiResourceStore : ITenantResourceStore<ApiResource>
    {
        Task<IReadOnlyList<ApiResource>> GetByScopesAsync(
            Guid scope,
            CancellationToken cancellationToken);
    }
}
