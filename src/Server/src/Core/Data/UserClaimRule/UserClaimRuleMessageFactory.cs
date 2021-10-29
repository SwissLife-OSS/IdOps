using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps
{
    public class UserClaimRuleMessageFactory : TenantResourceMessageFactory<Model.UserClaimRule>
    {
        public UserClaimRuleMessageFactory(IIdentityServerService identityServerService)
            : base(identityServerService)
        {
        }

        public override async ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            Model.UserClaimRule rule,
            CancellationToken cancellationToken) =>
            new IdentityServer.Model.UserClaimRule
            {
                Name = rule.Name,
                ClientIds =
                    rule.ApplicationId is null
                        ? Array.Empty<string>()
                        : await context.GetClientIdsOfApplicationsAsync(
                            new[]
                            {
                                rule.ApplicationId.Value
                            },
                            cancellationToken),
                Tenant = rule.Tenant,
                Rules = rule.Rules
                    .Where(x =>
                        x.EnvironmentId == null || x.EnvironmentId.Value == context.EnvironmentId)
                    .Select(x => new IdentityServer.Model.ClaimRuleMatch
                    {
                        ClaimType = x.ClaimType,
                        Value = x.Value,
                        MatchMode = PublisherHelper.MapEnum<IdentityServer.Model.ClaimRuleMatchMode>(x.MatchMode)
                    }),
                Claims = rule.Claims.Select(x =>
                    new IdentityServer.Model.UserClaim { Type = x.Type, Value = x.Value }
                ),
                Source = PublisherHelper.CreateSource(rule)
            };
    }
}
