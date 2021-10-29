using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IDependencyService
    {
        Task<Dependency> GetAllDependenciesAsync(GetDependenciesRequest input, CancellationToken cancellationToken);
    }
}
