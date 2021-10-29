using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public class ApiResourceDependencyResolver : ResourceDependencyResolver<ApiResource>
    {
        private readonly IResourceAuthoring _resourceAuthoring;

        public ApiResourceDependencyResolver(IResourceAuthoring resourceAuthoring)
        {
            _resourceAuthoring = resourceAuthoring;
        }

        public override async Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            IEnumerable<IResource> deps = await _resourceAuthoring.ApiResources
                .GetDependenciesAsync(id, cancellationToken);

            return deps.ToArray();
        }
    }
}
