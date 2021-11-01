using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static MongoDB.Driver.Builders<IdOps.Model.UserClaimRule>;

namespace IdOps.Server.Storage.Mongo
{
    public class UserClaimRulesStore : TenantResourceStore<UserClaimRule>, IUserClaimRuleStore
    {
        public UserClaimRulesStore(IIdOpsDbContext dbContext) : base(dbContext.UserClaimRules)
        {
        }

        public async Task<IReadOnlyList<UserClaimRule>> GetByApplicationsAsync(
            IEnumerable<Guid> applicationIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<UserClaimRule> filter =
                Filter.In(x => x.ApplicationId, applicationIds.Cast<Guid?>());
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
