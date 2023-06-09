using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public class ClientStore : IClientStore, IManageClientStore
    {
        private readonly IClientRepository _clientRepository;

        public ClientStore(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client?> FindClientByIdAsync(string clientId)
        {
            IdOpsClient? client = await _clientRepository.GetAsync(clientId, default);

            return client;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsClient client,
            CancellationToken cancellationToken)
        {
            return await _clientRepository.UpdateAsync(client, cancellationToken);
        }
    }
}
