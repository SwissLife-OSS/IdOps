using System;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Security;
using IdOps.Store;

namespace IdOps
{
    public interface IResourceManager<T> where T : class, IResource, new()
    {
        IResource? Original { get; }
        IResourceStore<T>? Store { get; }
        IUserContext UserContext { get; }

        T CreateNew();
        Task<T> GetExistingOrCreateNewAsync(Guid? id, CancellationToken cancellationToken);
        Task<SaveResourceResult<T>> SaveAsync(T resource, CancellationToken cancellationToken);
        void SetNewVersion(T resource);
    }
}
