using IdOps.Server.Storage.Mongo;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;

namespace IdOps.GraphQL.Tests
{
    public class RepositoryTest
    {
        protected readonly MongoResource MongoResource;
        protected IdOpsDbContext DbContext { get; }

        public RepositoryTest(MongoResource mongoResource)
        {
            MongoResource = mongoResource;
            DbContext = CreateDbContext();
        }

        protected virtual IdOpsDbContext CreateDbContext()
        {
            IMongoDatabase? db = MongoResource.CreateDatabase();

            return new IdOpsDbContext(new MongoOptions
            {
                ConnectionString = MongoResource.ConnectionString,
                DatabaseName = db.DatabaseNamespace.DatabaseName
            }, default);
        }
    }
}
