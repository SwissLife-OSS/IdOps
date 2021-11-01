using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public interface IApiResourceRepository
    {
        Task<IEnumerable<IdOpsApiResource>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<IdOpsApiResource>> GetByNameAsync(IEnumerable<string> names, CancellationToken cancellationToken);
        Task<IEnumerable<IdOpsApiResource>> GetByScopeNameAsync(IEnumerable<string> scopeNames, CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateAsync(IdOpsApiResource apiResource, CancellationToken cancellationToken);
    }

    public interface IManageClientStore
    {
        Task<UpdateResourceResult> UpdateAsync(
            IdOpsClient client,
            CancellationToken cancellationToken);
    }

    public interface IManageResourceStore
    {
        Task<UpdateResourceResult> UpdateIdentityResourceAsync(
            IdOpsIdentityResource resource,
            CancellationToken cancellationToken);

        Task<UpdateResourceResult> UpdateApiResourceAsync(
             IdOpsApiResource resource,
             CancellationToken cancellationToken);

        Task<UpdateResourceResult> UpdateApiScopeAsync(
            IdOpsApiScope scope,
            CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateUserClaimRuleAsync(UserClaimRule rule, CancellationToken cancellationToken);

        Task<UpdateResourceResult> UpdatePersonalAccessTokenAsync(
            IdOpsPersonalAccessToken token,
            CancellationToken cancellationToken);
    }
}
