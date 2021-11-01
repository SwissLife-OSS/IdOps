using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class UserClaimRulesService
        : TenantResourceService<UserClaimRule>, IUserClaimRulesService
    {
        private readonly IUserClaimRuleStore _store;
        private readonly IResourceManager<UserClaimRule> _resourceManager;

        public UserClaimRulesService(
            IUserClaimRuleStore store,
            IResourceManager<UserClaimRule> resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(userContextAccessor, store)
        {
            _store = store;
            _resourceManager = resourceManager;
        }

        public bool ForcePublish => true;

        public Task<IReadOnlyList<UserClaimRule>> GetByApplicationAsync(
            Guid applicationId,
            CancellationToken cancellationToken)
        {
            return _store.GetByApplicationsAsync(new[]
                {
                    applicationId
                },
                cancellationToken);
        }

        public async Task<UserClaimRule> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _store.GetByIdAsync(id, cancellationToken);
        }

        public async Task<UserClaimRule> SaveAsync(
            SaveUserClaimRuleRequest request,
            CancellationToken cancellationToken)
        {
            UserClaimRule rule =
                await _resourceManager.GetExistingOrCreateNewAsync(request.Id, cancellationToken);

            rule.Tenant = request.Tenant;
            rule.Name = request.Name;
            rule.Claims = request.Claims;
            rule.Rules = request.Rules;
            rule.ApplicationId = request.ApplicationId;

            SaveResourceResult<UserClaimRule> result =
                await _resourceManager.SaveAsync(rule, cancellationToken);

            return result.Resource;
        }
    }
}
