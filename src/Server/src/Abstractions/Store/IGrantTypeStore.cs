using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IGrantTypeStore
    {
        Task<IEnumerable<GrantType>> GetAllAsync(CancellationToken cancellationToken);
        Task<GrantType> SaveAsync(GrantType grantType, CancellationToken cancellationToken);
    }
}
