using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnyDiff;
using AnyDiff.Extensions;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    //public interface IResourceManager<T> : IResourceManager where T : class, IResource, new()
    //{
    //}

    public interface IResourceManager
    {
        ResourceChangeContext<T> CreateNew<T>() where T : IResource, new();

        Task<ResourceChangeContext<T>> GetExistingOrCreateNewAsync<T>(
            Guid? id,
            CancellationToken cancellationToken)
            where T : class, IResource, new();

        Task<SaveResourceResult<T>> SaveAsync<T>(
            ResourceChangeContext<T> context,
            CancellationToken cancellationToken)
            where T : class, IResource, new();

        ResourceChangeContext<T> SetNewVersion<T>(T resource)
            where T : class, IResource, new();
    }

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
