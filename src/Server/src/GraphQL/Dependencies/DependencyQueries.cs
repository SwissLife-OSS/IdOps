using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class DependencyQueries
    {
        private readonly IDependencyService _dependencyService;

        public DependencyQueries(
            IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public async Task<Dependency> GetDependenciesAsync(
            GetDependenciesRequest input,
            CancellationToken cancellationToken)
        {
            Dependency resources = await _dependencyService.GetAllDependenciesAsync(
                input,
                cancellationToken);
            return resources;
        }
    }
}
