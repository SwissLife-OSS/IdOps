using System;
using System.Collections.Generic;
using AnyDiff;
using AnyDiff.Extensions;

namespace IdOps
{
    public record ResourceChangeContext<T>(T Resource, T? _original)
    {
        public static ResourceChangeContext<T> FromNew(T resource) =>
            new(resource, default);

        private readonly T? _original = _original;

        private readonly Lazy<ICollection<Difference>> _diff =
            new(() => _original?.Diff(Resource) ?? Array.Empty<Difference>());

        public ICollection<Difference> Diff => _diff.Value;

        public bool HasExistingResource => _original is not null;
    }
}
