using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Server.Storage;

namespace IdOps
{
    public interface IIdentityServerService
    {
        Task<IEnumerable<Model.IdentityServer>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerGroup>> GetAllGroupsAsync(CancellationToken cancellationToken);
        Task<Model.IdentityServer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<string> GetDiscoveryDocumentAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityServerGroup> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByTenantAsync(string tenant, CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerGroup>> GetGroupsByUserTenants(CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerKey>> GetKeysAsync(Guid id, CancellationToken cancellationToken);
        Task<Model.IdentityServer> SaveAsync(SaveIdentityServerRequest request, CancellationToken cancellationToken);
    }

    public record SaveIdentityServerRequest(
        string Name,
        Guid EnvironmentId,
        Guid GroupId,
        string Url)
    {
        public Guid? Id { get; init; }
    }

    public record IdentityServerKey(string Kid, string Alg)
    {
        public string? Thumbprint { get; init; }
        public string? SerialNumber { get; init; }
        public string? Subject { get; init; }
        public DateTime? ValidUntil { get; init; }
    }
}
