using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public class ApplicationDependencyResolver : ResourceDependencyResolver<Application>
    {
        private readonly IResourceAuthoring _resourceAuthoring;

        public ApplicationDependencyResolver(IResourceAuthoring resourceAuthoring)
        {
            _resourceAuthoring = resourceAuthoring;
        }

        public override async Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Application? application = await _resourceAuthoring.Applications.GetByIdAsync(id, cancellationToken);
            if (application is not null)
            {
                IReadOnlyList<UserClaimRule> userClaimRules = await _resourceAuthoring.UserClaimRules
                    .GetByApplicationAsync(application.Id, cancellationToken);

                IReadOnlyList<Client> clients = await _resourceAuthoring.Clients
                    .GetManyAsync(application.ClientIds, cancellationToken);

                return userClaimRules
                    .Where(userClaimRule => userClaimRule.Rules.Any(rule =>
                        rule.IsGlobal() ||
                        rule.EnvironmentId.HasValue &&
                        clients.Any(client => client.Environments.Contains(rule.EnvironmentId.Value))))
                    .ToList();
            }

            return Array.Empty<IResource>();
        }
    }
}
