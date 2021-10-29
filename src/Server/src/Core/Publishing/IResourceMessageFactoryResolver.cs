using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IdOps
{
    public interface IResourceMessageFactoryResolver
    {
        IReadOnlyList<string> AvailableResourceTypes { get; }

        bool TryResolverFactory(
            string resourceType,
            [NotNullWhen(true)] out IResourceMessageFactory? value);
    }
}
