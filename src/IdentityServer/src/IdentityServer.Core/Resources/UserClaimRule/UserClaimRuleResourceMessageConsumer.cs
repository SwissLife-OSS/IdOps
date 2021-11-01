using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Storage;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class UserClaimRuleMessageConsumer : ResourceMessageConsumer<UserClaimRule>
    {
        private readonly IUserClaimRuleRepository _repository;

        public UserClaimRuleMessageConsumer(IUserClaimRuleRepository clientRepository)
        {
            _repository = clientRepository;
        }

        public override string MessageType => nameof(UserClaimRule);

        public override async Task<UpdateResourceResult> Process(
            UserClaimRule data,
            CancellationToken cancellationToken) =>
            await _repository.UpdateAsync(data, cancellationToken);
    }
}
