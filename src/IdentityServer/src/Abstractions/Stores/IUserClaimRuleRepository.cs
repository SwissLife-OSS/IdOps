using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public interface IUserClaimRuleRepository
    {
        Task<IEnumerable<UserClaimRule>> GetAllAsync(CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateAsync(UserClaimRule apiResource, CancellationToken cancellationToken);
    }
}
