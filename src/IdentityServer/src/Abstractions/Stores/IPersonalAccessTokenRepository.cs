using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public interface IPersonalAccessTokenRepository
    {
        Task<IdOpsPersonalAccessToken> CreateAsync(
            IdOpsPersonalAccessToken pat,
            CancellationToken cancellationToken);

        Task<IdOpsPersonalAccessToken> SaveAsync(
            IdOpsPersonalAccessToken pat,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IdOpsPersonalAccessToken>> GetActiveTokensByUserNameAsync(
            string userName,
            CancellationToken cancellationToken);

        Task<IdOpsPersonalAccessToken?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<UpdateResourceResult> UpdateResourceAsync(
            IdOpsPersonalAccessToken apiResource,
            CancellationToken cancellationToken);
    }
}
