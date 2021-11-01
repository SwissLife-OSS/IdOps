using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IResourceApprovalStateStore
    {
        Task<IReadOnlyList<ResourceApprovalState>> GetAllAsync(CancellationToken cancellationToken);
        Task<ResourceApprovalState> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<ResourceApprovalState>> GetManyAsync(
            IEnumerable<Guid> resourceIds,
            CancellationToken cancellationToken);

        Task<ResourceApprovalState> SaveAsync(
            ResourceApprovalState publishState,
            CancellationToken cancellationToken);
    }
}
