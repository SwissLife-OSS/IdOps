using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;

namespace IdOps.IdentityServer.Store.Mongo.Tests
{
    public class RepositoryTest
    {
        protected readonly MongoResource MongoResource;
        protected IdentityStoreDbContext DbContext { get; }

        public RepositoryTest(MongoResource mongoResource)
        {
            MongoResource = mongoResource;
            DbContext = CreateDbContext();
        }

        protected virtual IdentityStoreDbContext CreateDbContext()
        {
            IMongoDatabase? db = MongoResource.CreateDatabase();

            return new IdentityStoreDbContext(new IdOpsMongoOptions
            {
                ConnectionString = MongoResource.ConnectionString,
                DatabaseName = db.DatabaseNamespace.DatabaseName
            });
        }
    }
}
