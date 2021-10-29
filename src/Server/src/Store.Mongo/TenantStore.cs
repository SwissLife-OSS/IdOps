using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;

namespace IdOps.Store.Mongo
{
    public class TenantStore : ITenantStore
    {
        public TenantStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.Tenants;
        }

        protected IMongoCollection<Tenant> Collection { get; }

        public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken)
        {
            FilterDefinition<Tenant> filter = FilterDefinition<Tenant>.Empty;

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Tenant>> GetManyAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Tenant> filter = Builders<Tenant>.Filter.In(x => x.Id, ids);

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<Tenant> SaveAsync(
            Tenant tenant,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Tenant>? filter = Builders<Tenant>.Filter.Eq(x => x.Id, tenant.Id);
            ReplaceOptions options = new() { IsUpsert = true };
            await Collection.ReplaceOneAsync(filter, tenant, options, cancellationToken);

            return tenant;
        }
    }
}
