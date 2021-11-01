using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.Environment>;
using Environment = IdOps.Model.Environment;

namespace IdOps.Server.Storage.Mongo
{
    public class EnvironmentStore : IEnvironmentStore
    {
        public EnvironmentStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.Environments;
        }

        protected IMongoCollection<Environment> Collection { get; }

        public async Task<IReadOnlyList<Environment>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            FilterDefinition<Environment> filter = FilterDefinition<Environment>.Empty;
            SortDefinition<Environment> sort = Sort.Ascending(x => x.Order);

            return await Collection.Find(filter).Sort(sort).ToListAsync(cancellationToken);
        }


        public async Task<Environment> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Environment> filter = Filter.Eq(x => x.Id, id);
            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<Environment> SaveAsync(
            Environment environment,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Environment> filter = Filter.Eq(x => x.Id, environment.Id);
            ReplaceOptions options = new() { IsUpsert = true };

            await Collection.ReplaceOneAsync(filter, environment, options, cancellationToken);

            return environment;
        }
    }
}
