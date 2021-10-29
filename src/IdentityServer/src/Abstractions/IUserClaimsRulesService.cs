using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Services
{
    public interface IUserClaimsRulesService
    {
        Task<IReadOnlyList<UserClaimRule>> GetRulesByClientAsync(
            IdOpsClient client,
            IEnumerable<string> claimTypes,
            CancellationToken cancellationToken);
    }
}
