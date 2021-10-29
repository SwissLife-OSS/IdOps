using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class IdentityResourceMessageConsumer
        : ResourceMessageConsumer<IdOpsIdentityResource>
    {
        private readonly IIdentityResourceRepository _repository;

        public IdentityResourceMessageConsumer(IIdentityResourceRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => nameof(IdentityResource);

        public override async Task<UpdateResourceResult> Process(
            IdOpsIdentityResource data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateAsync(data, cancellationToken);
    }
}
