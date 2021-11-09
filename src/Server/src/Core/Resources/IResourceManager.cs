using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
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
}
