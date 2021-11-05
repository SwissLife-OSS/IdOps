using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IIdentityServerService
    {
        Task<IReadOnlyList<Model.IdentityServer>> GetAllAsync(CancellationToken cancellationToken);
        Task<Model.IdentityServer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<string> GetDiscoveryDocumentAsync(Guid id, CancellationToken cancellationToken);
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
