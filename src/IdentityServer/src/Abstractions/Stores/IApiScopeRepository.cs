using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Storage
{
    public interface IApiScopeRepository
    {
        Task<IEnumerable<ApiScope>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<ApiScope>> GetByNameAsync(IEnumerable<string> scopeNames, CancellationToken cancellationToken);
        Task<UpdateResourceResult> UpdateAsync(IdOpsApiScope apiScope, CancellationToken cancellationToken);
    }
}
