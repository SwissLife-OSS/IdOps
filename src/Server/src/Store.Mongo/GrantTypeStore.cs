using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.GrantType>;

namespace IdOps.Server.Storage.Mongo
{
    public class GrantTypeStore : IGrantTypeStore
    {
        public GrantTypeStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.GrantTypes;
        }

        protected IMongoCollection<GrantType> Collection { get; }

        public async Task<IEnumerable<GrantType>> GetAllAsync(CancellationToken cancellationToken)
        {
            FilterDefinition<GrantType> filter = FilterDefinition<GrantType>.Empty;

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<GrantType> SaveAsync(
            GrantType grantType,
            CancellationToken cancellationToken)
        {
            FilterDefinition<GrantType> filter = Filter.Eq(x => x.Id, grantType.Id);

            ReplaceOptions options = new() { IsUpsert = true };

            await Collection.ReplaceOneAsync(filter, grantType, options, cancellationToken);

            return grantType;
        }
    }
}
