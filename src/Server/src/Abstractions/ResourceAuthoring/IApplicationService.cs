using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IApplicationService
    {
        Task<ApplicationWithClients> CreateAsync(CreateApplicationRequest request, CancellationToken cancellationToken);
        Task<Application?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Application>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<Application?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<SearchResult<Application>> SearchAsync(SearchApplicationsRequest request, CancellationToken cancellationToken);
        Task<Application> UpdateAsync(UpdateApplicationRequest request, CancellationToken cancellationToken);
        Task<Application> RemoveClientAsync(RemoveClientRequest input, CancellationToken cancellationToken);
        Task<Application> AddClientAsync(AddClientRequest input, CancellationToken cancellationToken);
        Task<IEnumerable<Client>> SearchUnMappedClientsAsync(string tenant, CancellationToken cancellationToken);
        Task<ApplicationWithClients> AddEnvironmentToApplicationAsnyc(AddEnvironmentToApplicationRequest request, CancellationToken cancellationToken);
    }

    public record ApplicationWithClients(
        Application Application,
        IEnumerable<CreatedClientInfo> Clients);

    public record CreatedClientInfo(Guid Id, string ClientId, string Name)
    {
        public string? SecretValue { get; init; }
    }
}
