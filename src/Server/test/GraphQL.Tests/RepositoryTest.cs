using IdOps.Server.Storage.Mongo;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;

namespace IdOps.GraphQL.Tests;

public class RepositoryTest
{
    public RepositoryTest(MongoResource mongoResource)
    {
        MongoResource = mongoResource;
        DbContext = CreateDbContext();
    }

    protected MongoResource MongoResource { get; }

    protected IdOpsDbContext DbContext { get; }

    protected virtual IdOpsDbContext CreateDbContext()
    {
        IMongoDatabase db = MongoResource.CreateDatabase();

        var options = new MongoOptions
        {
            ConnectionString = MongoResource.ConnectionString,
            DatabaseName = db.DatabaseNamespace.DatabaseName
        };

        return new IdOpsDbContext(options, default);
    }
}
