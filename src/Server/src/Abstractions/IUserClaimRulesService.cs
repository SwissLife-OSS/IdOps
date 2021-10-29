using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IUserClaimRulesService : IResourceService<UserClaimRule>
    {
        Task<IReadOnlyList<UserClaimRule>> GetByApplicationAsync(Guid applicationId, CancellationToken cancellationToken);
        Task<UserClaimRule> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<UserClaimRule> SaveAsync(SaveUserClaimRuleRequest request, CancellationToken cancellationToken);
    }
}
