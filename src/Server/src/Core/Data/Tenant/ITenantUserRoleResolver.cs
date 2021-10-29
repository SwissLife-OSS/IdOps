using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface ITenantUserRoleResolver
    {
        Task<IReadOnlyDictionary<string, IList<TenantRole>>> GetClaimRoleMappingsAsync(CancellationToken cancellationToken);
    }
}