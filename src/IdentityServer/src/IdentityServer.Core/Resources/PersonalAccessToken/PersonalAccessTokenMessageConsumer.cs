using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class PersonalAccessTokenMessageConsumer
        : ResourceMessageConsumer<IdOpsPersonalAccessToken>
    {
        private readonly IPersonalAccessTokenRepository _repository;

        public PersonalAccessTokenMessageConsumer(IPersonalAccessTokenRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => "PersonalAccessToken";

        public override async Task<UpdateResourceResult> Process(
            IdOpsPersonalAccessToken data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateResourceAsync(data, cancellationToken);
    }
}
