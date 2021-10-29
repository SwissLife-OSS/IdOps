using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IdOps
{
    public class ResourceServiceResolver
        : IResourceServiceResolver
    {
        private readonly IReadOnlyDictionary<string, IResourceService> _lookupByResourceType;

        public ResourceServiceResolver(IEnumerable<IResourceService> resourceServices)
        {
            _lookupByResourceType = resourceServices.ToDictionary(x => x.ResourceType);
            AvailableResourceTypes = _lookupByResourceType.Keys.ToArray();
        }

        public IReadOnlyList<string> AvailableResourceTypes { get; }

        public bool ContainsResource(string resourceType) =>
            _lookupByResourceType.ContainsKey(resourceType);

        public bool TryResolveService(
            string resourceType,
            [NotNullWhen(true)] out IResourceService? value) =>
            _lookupByResourceType.TryGetValue(resourceType, out value);
    }
}
