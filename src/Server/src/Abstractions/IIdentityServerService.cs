using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IIdentityServerService
    {
        Task<IReadOnlyList<Model.IdentityServer>> GetAllAsync(CancellationToken cancellationToken);
        Task<Model.IdentityServer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<string> GetDiscoveryDocumentAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerKey>> GetKeysAsync(Guid id, CancellationToken cancellationToken);
        Task<Model.IdentityServer> SaveAsync(SaveIdentityServerRequest request, CancellationToken cancellationToken);
    }
}
