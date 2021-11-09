using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public abstract class TenantResourceMessageFactory<T> : ResourceMessageFactory<T>
        where T : class, IResource, IHasTenant, new()
    {
        private readonly IIdentityServerGroupService _identityServerGroupService;

        public TenantResourceMessageFactory(IIdentityServerGroupService identityServerGroupService)
        {
            _identityServerGroupService = identityServerGroupService;
        }

        public override async ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            T resource,
            CancellationToken cancellationToken) =>
            await _identityServerGroupService
                .GetGroupByTenantAsync(resource.Tenant, cancellationToken);
    }
}
