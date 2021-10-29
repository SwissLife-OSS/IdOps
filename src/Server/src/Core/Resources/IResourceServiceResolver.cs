using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IdOps
{
    public interface IResourceServiceResolver
    {
        IReadOnlyList<string> AvailableResourceTypes { get; }

        bool ContainsResource(string resourceType);

        bool TryResolveService(
            string resourceType,
            [NotNullWhen(true)] out IResourceService? value);
    }
}
