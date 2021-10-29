using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IPublishingContext
    {
        Guid EnvironmentId { get; }

        ApiResource? GetApiResourceById(Guid id);

        IdentityResource? GetIdentityResourceById(Guid id);

        ApiScope? GetApiScopeById(Guid id);

        Model.Environment? GetEnvironmentById(Guid id);

        Task<ICollection<string>> GetClientIdsOfApplicationsAsync(
            IEnumerable<Guid> applicationIds,
            CancellationToken cancellationToken);
    }
}
