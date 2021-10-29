using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class ClientMessageConsumer : ResourceMessageConsumer<IdOpsClient>
    {
        private readonly IClientRepository _repository;

        public ClientMessageConsumer(IClientRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => nameof(Client);

        public override async Task<UpdateResourceResult> Process(
            IdOpsClient data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateAsync(data, cancellationToken);
    }
}
