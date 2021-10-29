using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public abstract class TenantResourceMessageFactory<T> : ResourceMessageFactory<T>
        where T : class, IResource, IHasTenant, new()
    {
        private readonly IIdentityServerService _identityServerService;

        public TenantResourceMessageFactory(IIdentityServerService identityServerService)
        {
            _identityServerService = identityServerService;
        }

        public override async ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            T resource,
            CancellationToken cancellationToken) =>
            await _identityServerService
                .GetGroupByTenantAsync(resource.Tenant, cancellationToken);
    }
}
