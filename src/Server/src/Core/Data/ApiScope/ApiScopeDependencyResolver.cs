using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public class ApiScopeDependencyResolver : ResourceDependencyResolver<ApiScope>
    {
        private readonly IResourceAuthoring _resourceAuthoring;

        public ApiScopeDependencyResolver(IResourceAuthoring resourceAuthoring)
        {
            _resourceAuthoring = resourceAuthoring;
        }

        public override async Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            ApiScope? apiScope = await _resourceAuthoring.ApiScopes
                .GetByIdAsync(id, cancellationToken);

            if (apiScope is not null)
            {
                var dependencies = new List<IResource>();
                dependencies.AddRange(await GetApiScopesAsync(apiScope.AllowedScopes, cancellationToken));
                dependencies.AddRange(await GetIdentityScopesAsync(client.AllowedScopes, cancellationToken));
                dependencies.AddRange(await GetRulesAsync(client, cancellationToken));

                return dependencies;
            }

            return Array.Empty<IResource>();
        }

        private async Task<IEnumerable<IResource>> GetApiScopesAsync(
            ICollection<ClientScope> allowedScopes,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid> resourceScopeIds = allowedScopes
                .Where(s => s.Type == ScopeType.Resource)
                .Select(s => s.Id);

            return await _resourceAuthoring.ApiScopes.GetByIdsAsync(resourceScopeIds, cancellationToken);
        }

        private async Task<IEnumerable<IResource>> GetIdentityScopesAsync(
            ICollection<ClientScope> allowedScopes,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid> identityScopeIds = allowedScopes
                .Where(s => s.Type == ScopeType.Identity)
                .Select(s => s.Id);

            return await _resourceAuthoring.IdentityResources.GetByIdsAsync(identityScopeIds, cancellationToken);
        }

        private async Task<IEnumerable<IResource>> GetRulesAsync(
            Client client,
            CancellationToken cancellationToken)
        {
            Application? application = await _resourceAuthoring.Applications
                .GetByClientIdAsync(client.Id, cancellationToken);

            if (application == null)
            {
                return Enumerable.Empty<IResource>();
            }

            IReadOnlyList<UserClaimRule> userClaimRules = await _resourceAuthoring.UserClaimRules
                .GetByApplicationAsync(application.Id, cancellationToken);

            return userClaimRules
                .Where(userClaimRule => userClaimRule.Rules.Any(rule =>
                    rule.IsGlobal() ||
                    rule.EnvironmentId.HasValue &&
                    client.Environments.Contains(rule.EnvironmentId.Value)));
        }
    }
}
