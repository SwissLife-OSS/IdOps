using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IEnvironmentStore
    {
        Task<IReadOnlyList<Environment>> GetAllAsync(CancellationToken cancellationToken);
        Task<Environment> GetByIdAsync(System.Guid id, CancellationToken cancellationToken);
        Task<Environment> SaveAsync(Environment environment, CancellationToken cancellationToken);
    }
}
