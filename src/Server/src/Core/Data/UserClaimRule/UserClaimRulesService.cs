using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Configuration;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class UserClaimRulesService
        : TenantResourceService<UserClaimRule>, IUserClaimRulesService
    {
        private readonly IUserClaimRuleStore _store;
        private readonly IResourceManager _resourceManager;

        public UserClaimRulesService(
            IdOpsServerOptions options,
            IUserClaimRuleStore store,
            IResourceManager resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(options, userContextAccessor, store)
        {
            _store = store;
            _resourceManager = resourceManager;
        }

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

        public async Task<UserClaimRule> SaveAsync(
            SaveUserClaimRuleRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<UserClaimRule> context = await _resourceManager
                .GetExistingOrCreateNewAsync<UserClaimRule>(request.Id, cancellationToken);

            context.Resource.Tenant = request.Tenant;
            context.Resource.Name = request.Name;
            context.Resource.Claims = request.Claims;
            context.Resource.Rules = request.Rules;
            context.Resource.ApplicationId = request.ApplicationId;

            SaveResourceResult<UserClaimRule> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public override bool IsAllowedToPublish()
        {
            return UserContext.HasPermission(
                Permissions.ClientAuthoring.Publish);
        }

        public override bool IsAllowedToApprove()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.Approve);
        }
    }
}
