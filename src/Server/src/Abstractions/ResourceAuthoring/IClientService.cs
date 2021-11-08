using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IClientService : IResourceService<Client>
    {
        Task<(Client, string)> AddClientSecretAsync(AddClientSecretRequest request, CancellationToken cancellationToken);
        Task<Client> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken);
        Task<Client> CreateClientAsync(Client client, CancellationToken cancellationToken);
        Task<Client> GetClientByIdAsync(Guid id, CancellationToken cancellationToken);
        IReadOnlyList<string> GetClientIdGenerators();
        Task<IReadOnlyList<Client>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        IReadOnlyList<string> GetSharedSecretGenerators();
        Task<Client> RemoveClientSecretAsync(RemoveClientSecretRequest request, CancellationToken cancellationToken);
        Task<SearchResult<Client>> SearchClientsAsync(SearchClientsRequest request, CancellationToken cancellationToken);
        Task<Client> UpdateClientAsync(UpdateClientRequest request, CancellationToken cancellationToken);
        Task<Client> UpdateClientAsync(Client client, CancellationToken cancellationToken);
    }
}
