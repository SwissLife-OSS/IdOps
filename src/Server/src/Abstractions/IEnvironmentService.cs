using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IEnvironmentService
    {
        Task<IReadOnlyList<Environment>> GetAllAsync(CancellationToken cancellationToken);
        Task<Environment> GetByIdAsync(System.Guid id, CancellationToken cancellationToken);
        Task<Environment?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<Environment> SaveAsync(SaveEnvironmentRequest request, CancellationToken cancellationToken);
    }
}
