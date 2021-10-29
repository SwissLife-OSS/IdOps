using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class UserClaimRuleRepository : IUserClaimRuleRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public UserClaimRuleRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserClaimRule>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            List<UserClaimRule> rules = await _dbContext.UserClaimRules
                .AsQueryable()
                .ToListAsync(cancellationToken);

            return rules;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            UserClaimRule apiResource,
            CancellationToken cancellationToken)
        {
            var updater = new ResourceUpdater<UserClaimRule>(_dbContext.UserClaimRules);

            return await updater.UpdateAsync(apiResource, cancellationToken);
        }
    }
}

