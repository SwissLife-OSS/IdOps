using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IResourcePublishStateStore
    {
        Task<IEnumerable<ResourcePublishState>> GetAllAsync(CancellationToken cancellationToken);
        Task<ResourcePublishState> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<ResourcePublishState>> GetManyAsync(IEnumerable<Guid> resourceIds, CancellationToken cancellationToken);
        Task<ResourcePublishState> SaveAsync(ResourcePublishState publishState, CancellationToken cancellationToken);
    }
}
