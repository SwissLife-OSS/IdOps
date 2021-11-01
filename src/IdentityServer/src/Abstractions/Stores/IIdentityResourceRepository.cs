using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public interface IIdentityResourceRepository
    {
        Task<IEnumerable<IdOpsIdentityResource>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<IEnumerable<IdOpsIdentityResource>> GetByNameAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateAsync(IdOpsIdentityResource identityResource, CancellationToken cancellationToken);
    }
}
