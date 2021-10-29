using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public class IdentityResourceMessageFactory : ResourceMessageFactory<IdentityResource>
    {
        private readonly IIdentityServerService _identityServerService;

        public IdentityResourceMessageFactory(IIdentityServerService identityServerService)
        {
            _identityServerService = identityServerService;
        }

        public override ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            IdentityResource identityResource,
            CancellationToken cancellationToken) =>
            new(new IdOpsIdentityResource
            {
                Description = identityResource.Description,
                DisplayName = identityResource.DisplayName,
                Emphasize = false,
                Enabled = identityResource.Enabled,
                Name = identityResource.Name,
                Required = false,
                ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                UserClaims = identityResource.UserClaims,
                Source = PublisherHelper.CreateSource(identityResource)
            });

        public override async ValueTask<IdentityServerGroup?> ResolveServerGroupAsync(
            IdentityResource resource,
            CancellationToken cancellationToken) =>
            await _identityServerService
                .GetGroupByIdAsync(
                    resource.IdentityServerGroupId,
                    cancellationToken);
    }
}
