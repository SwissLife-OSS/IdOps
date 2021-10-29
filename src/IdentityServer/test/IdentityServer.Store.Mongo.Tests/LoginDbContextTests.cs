using System;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using FluentAssertions;
using IdOps.IdentityServer.Store.Mongo;
using IdOps.IdentityServer.Store.Mongo.Tests;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Store.Mongo.Tests
{
    [Collection(TestCollectionNames.Store)]
    public class LoginDbContextTests
    {
        private readonly MongoResource _mongoResource;

        public LoginDbContextTests(MongoResource mongoResource)
        {
            _mongoResource = mongoResource;
        }

        [Fact]
        public async Task PersistentGrantCollection_WithExistingIndexHavingDifferentOptions_NoExecption()
        {
            // Arrange
            IMongoDatabase db = _mongoResource.CreateDatabase();

            var mongoOptions = new IdOpsMongoOptions
            {
                ConnectionString = _mongoResource.ConnectionString,
                DatabaseName = db.DatabaseNamespace.DatabaseName
            };

            IMongoCollection<PersistedGrant> collection =  db.GetCollection<PersistedGrant>("grant");

            var ttlIndex = new CreateIndexModel<PersistedGrant>(
                Builders<PersistedGrant>.IndexKeys.Ascending(c => c.CreationTime),
                new CreateIndexOptions
                {
                    Unique = false,
                    ExpireAfter = TimeSpan.FromDays(1)
                });

            await collection.Indexes.CreateOneAsync(ttlIndex);

            // Act
            Action action = () => new IdentityStoreDbContext(mongoOptions);

            // Assert
            action.Should().NotThrow();
        }
        
    }
}
