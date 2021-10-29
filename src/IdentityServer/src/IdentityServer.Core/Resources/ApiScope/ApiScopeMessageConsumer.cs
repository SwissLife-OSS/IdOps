using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class ApiScopeMessageConsumer : ResourceMessageConsumer<IdOpsApiScope>
    {
        private readonly IApiScopeRepository _repository;

        public ApiScopeMessageConsumer(IApiScopeRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => nameof(ApiScope);

        public override async Task<UpdateResourceResult> Process(
            IdOpsApiScope data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateAsync(data, cancellationToken);
    }
}
