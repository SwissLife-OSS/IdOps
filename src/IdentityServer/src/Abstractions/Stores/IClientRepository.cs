using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Store
{
    public interface IClientRepository
    {
        Task<HashSet<string>> GetAllClientOrigins();
        Task<HashSet<string>> GetAllClientRedirectUriAsync();
        Task<IdOpsClient?> GetAsync(string id, CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateAsync(IdOpsClient client, CancellationToken cancellationToken);
    }
}
