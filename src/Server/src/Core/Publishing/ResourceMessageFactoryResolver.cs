using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IdOps
{
    public class ResourceMessageFactoryResolver
        : IResourceMessageFactoryResolver
    {
        private readonly IReadOnlyDictionary<string, IResourceMessageFactory> _lookupByResourceType;

        public ResourceMessageFactoryResolver(IEnumerable<IResourceMessageFactory> resourceServices)
        {
            _lookupByResourceType = resourceServices.ToDictionary(x => x.ResourceType);
            AvailableResourceTypes = _lookupByResourceType.Keys.ToArray();
        }

        public IReadOnlyList<string> AvailableResourceTypes { get; }

        public bool TryResolverFactory(
            string resourceType,
            [NotNullWhen(true)] out IResourceMessageFactory? value) =>
            _lookupByResourceType.TryGetValue(resourceType, out value);
    }
}
