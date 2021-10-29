using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IGrantTypeService
    {
        Task<GrantType> SaveAsync(SaveGrantTypeRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<GrantType>> GetAllAsync(CancellationToken cancellationToken);
    }
}
