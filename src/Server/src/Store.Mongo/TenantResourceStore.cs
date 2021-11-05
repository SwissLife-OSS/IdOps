using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdOps.Server.Storage.Mongo
{
    public abstract class TenantResourceStore<T> : ResourceStore<T>, ITenantResourceStore<T>
        where T : class, ITenantResource, new()
    {
        protected TenantResourceStore(IMongoCollection<T> collection) : base(collection)
        {
        }

        public override async Task<IReadOnlyList<T>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<T>? filter = FilterDefinition<T>.Empty;

            if (ids is not null)
            {
                filter &= Builders<T>.Filter.In(x => x.Id, ids);
            }

            if (tenants is not null)
            {
                filter &= Builders<T>.Filter.In(x => x.Tenant, tenants);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> SearchByTenantAsync(
            string tenant,
            CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Where(x => x.Tenant == tenant);

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken) =>
            GetAllAsync(ids, tenants, cancellationToken);
    }
}
