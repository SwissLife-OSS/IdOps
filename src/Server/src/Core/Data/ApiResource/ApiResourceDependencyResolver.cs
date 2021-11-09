using System;
using System.Collections.Generic;
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
            ApiResource? apiResource = await _resourceAuthoring.ApiResources
                .GetByIdAsync(id, cancellationToken);

            if (apiResource is not null)
            {
                return await _resourceAuthoring.ApiScopes
                    .GetByIdsAsync(apiResource.Scopes, cancellationToken);    
            }

            return Array.Empty<IResource>();
        }
    }
}
