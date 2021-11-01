using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IUserClaimRuleStore : ITenantResourceStore<UserClaimRule>
    {
        Task<IReadOnlyList<UserClaimRule>> GetByApplicationsAsync(
            IEnumerable<Guid> applicationIds,
            CancellationToken cancellationToken);
    }
}
