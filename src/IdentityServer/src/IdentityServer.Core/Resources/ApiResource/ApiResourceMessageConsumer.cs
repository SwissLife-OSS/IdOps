using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Storage;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class ApiResourceMessageConsumer : ResourceMessageConsumer<IdOpsApiResource>
    {
        private readonly IApiResourceRepository _repository;

        public ApiResourceMessageConsumer(IApiResourceRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => nameof(ApiResource);

        public override async Task<UpdateResourceResult> Process(
            IdOpsApiResource data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateAsync(data, cancellationToken);
    }
}
