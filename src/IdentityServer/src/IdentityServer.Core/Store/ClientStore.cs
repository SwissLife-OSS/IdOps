using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public class ClientStore : IClientStore, IManageClientStore
    {
        private readonly IClientRepository _clientRepository;
        private readonly IpWhitelistValidator _ipWhitelistValidator;
        private readonly IEventService _eventService;

        public ClientStore(
            IClientRepository clientRepository,
            IpWhitelistValidator ipWhitelistValidator,
            IEventService eventService)
        {
            _clientRepository = clientRepository;
            _ipWhitelistValidator = ipWhitelistValidator;
            _eventService = eventService;
        }

        public async Task<Client?> FindClientByIdAsync(string clientId)
        {
            IdOpsClient? client = await _clientRepository.GetAsync(clientId, default);

            if (client is null)
            {
                return null;
            }

            if (_ipWhitelistValidator.IsValid(client, out var message))
            {
                return client;
            }

            await IpValidationFailedEvent.New(clientId, message).RaiseAsync(_eventService);

            return null;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsClient client,
            CancellationToken cancellationToken)
        {
            return await _clientRepository.UpdateAsync(client, cancellationToken);
        }
    }
}
