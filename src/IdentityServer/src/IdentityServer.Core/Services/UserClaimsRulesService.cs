using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store;
using Microsoft.Extensions.Caching.Memory;

namespace IdOps.IdentityServer.Services
{
    public class UserClaimsRulesService : IUserClaimsRulesService
    {
        private readonly IUserClaimRuleRepository _claimRuleRepository;
        private readonly IMemoryCache _memoryCache;

        public UserClaimsRulesService(
            IUserClaimRuleRepository claimRuleRepository,
            IMemoryCache memoryCache)
        {
            _claimRuleRepository = claimRuleRepository;
            _memoryCache = memoryCache;
        }

        public async Task<IReadOnlyList<UserClaimRule>> GetAllRulesAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<UserClaimRule> rules = await _memoryCache.GetOrCreateAsync(
                "_uc_rules",
                async (entry) =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5);

                return await _claimRuleRepository.GetAllAsync(cancellationToken);
            });

            return rules.ToList();
        }

        public async Task<IReadOnlyList<UserClaimRule>> GetRulesByClientAsync(
            IdOpsClient client,
            IEnumerable<string> claimTypes,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<UserClaimRule>? allRules = await GetAllRulesAsync(cancellationToken);

            return allRules.Where(x =>
                x.ClientIds.Count() == 0 || x.ClientIds.Contains(client.ClientId) &&
                x.Tenant == client.Tenant &&
                x.Rules.Any(r => claimTypes.Contains(r.ClaimType)))
                .ToList();
        }
    }
}
