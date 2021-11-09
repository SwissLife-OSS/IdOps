using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public class ApiResourceMessageFactory : TenantResourceMessageFactory<ApiResource>
    {
        public ApiResourceMessageFactory(IIdentityServerGroupService identityServerGroupService)
            : base(identityServerGroupService)
        {
        }

        public override ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            ApiResource apiResource,
            CancellationToken cancellationToken) =>
            new(new IdOpsApiResource
            {
                AllowedAccessTokenSigningAlgorithms =
                    apiResource.AllowedAccessTokenSigningAlgorithms,
                Description = apiResource.Description,
                DisplayName = apiResource.DisplayName,
                Enabled = apiResource.Enabled,
                Name = apiResource.Name,
                RequireResourceIndicator = apiResource.RequireResourceIndicator,
                Scopes = context.GetAllowedScopes(apiResource.Scopes),
                ApiSecrets = apiResource.ApiSecrets?
                    .Select(s => new Duende.IdentityServer.Models.Secret(s.Value, s.Description, s.Expiration))
                    .ToArray() ?? Array.Empty<Duende.IdentityServer.Models.Secret>(),
                ShowInDiscoveryDocument = false,
                UserClaims = apiResource.UserClaims
            });
    }
}
