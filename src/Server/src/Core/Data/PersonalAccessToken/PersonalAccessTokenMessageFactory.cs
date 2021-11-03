using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;

namespace IdOps
{
    public class PersonalAccessTokenMessageFactory
        : TenantResourceMessageFactory<PersonalAccessToken>
    {
        public PersonalAccessTokenMessageFactory(IIdentityServerService identityServerService)
            : base(identityServerService)
        {
        }

        public override async ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            PersonalAccessToken token,
            CancellationToken cancellationToken) =>
            token.EnvironmentId == context.EnvironmentId
                ? new IdOpsPersonalAccessToken()
                {
                    Id = token.Id,
                    Tokens = token.Tokens
                        .Select(x =>
                            new IdOpsHashedToken(x.Id, x.Token, x.ExpiresAt, x.CreatedAt, false))
                        .ToList(),
                    CreatedAt = token.CreatedAt,
                    AllowedScopes = token.AllowedScopes,
                    ClaimExtensions = token.ClaimsExtensions
                        .Select(x => new ClaimExtension(x.Type, x.Value))
                        .ToArray(),
                    HashAlgorithm = token.HashAlgorithm,
                    TokenSource = token.Source,
                    UserName = token.UserName,
                    Source = PublisherHelper.CreateSource(token),
                    AllowedClients =
                        await context.GetClientIdsOfApplicationsAsync(
                            token.AllowedApplicationIds,
                            token.EnvironmentId,
                            cancellationToken)
                }
                : null;
    }
}
