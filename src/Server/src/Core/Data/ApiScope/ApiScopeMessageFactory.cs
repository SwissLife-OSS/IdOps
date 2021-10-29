using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public class ApiScopeMessageFactory : TenantResourceMessageFactory<ApiScope>
    {
        public ApiScopeMessageFactory(IIdentityServerService identityServerService)
            : base(identityServerService)
        {
        }

        public override ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            ApiScope apiScope,
            CancellationToken cancellationToken) =>
            new(new IdOpsApiScope
            {
                Description = apiScope.Description,
                DisplayName = apiScope.DisplayName,
                Emphasize = false,
                Enabled = apiScope.Enabled,
                Name = apiScope.Name,
                Required = false,
                ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                UserClaims = apiScope.UserClaims,
            });
    }
}
