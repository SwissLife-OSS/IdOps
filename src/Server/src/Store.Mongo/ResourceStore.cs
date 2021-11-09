using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdOps.Server.Storage.Mongo
{
    public abstract class ResourceStore<T> : IResourceStore<T>
        where T : class, IResource, new()
    {
        protected ResourceStore(IMongoCollection<T> collection)
        {
            Collection = collection;
            Type = typeof(T).Name;
        }

        protected IMongoCollection<T> Collection { get; }

        public string Type { get; }

        public abstract Task<IReadOnlyList<T>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        async Task<IReadOnlyList<IResource>> IResourceStore.GetByIdsAsync(
            IEnumerable<Guid>? ids,
            CancellationToken cancellationToken) =>
            await GetByIdsAsync(ids, cancellationToken);

        public Task<IReadOnlyList<T>> GetByIdsAsync(
            IEnumerable<Guid>? ids,
            CancellationToken cancellationToken) =>
            GetAllAsync(ids, null, cancellationToken);

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<T>? filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> SaveAsync(T resource, CancellationToken cancellationToken)
        {
            FilterDefinition<T>? filter = Builders<T>.Filter.Eq(x => x.Id, resource.Id);
            ReplaceOptions options = new() { IsUpsert = true };
            await Collection.ReplaceOneAsync(filter, resource, options, cancellationToken);
            return resource;
        }

        async Task<IReadOnlyList<IResource>> IResourceStore.GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken) =>
            await GetAllAsync(ids, tenants, cancellationToken);

        async Task<IResource> IResourceStore.GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            await GetByIdAsync(id, cancellationToken);

        async Task<IResource> IResourceStore.SaveAsync(
            IResource resource,
            CancellationToken cancellationToken)
        {
            if (resource is not T resourceOfT)
            {
                throw new ArgumentException($"Resource was not of type {Type}", nameof(resource));
            }

            return await SaveAsync(resourceOfT, cancellationToken);
        }
    }
}
