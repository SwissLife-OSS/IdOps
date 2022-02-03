using System;
using Xunit;
using Squadron;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.Server.Storage.Mongo;
using System.Collections.Generic;

namespace IdOps.Server.Store.Mongo.Tests
{
    public class ApiResourceStoreTests : IClassFixture<MongoResource>
    {
        private readonly MongoResource _mongoResource;

        public ApiResourceStoreTests(MongoResource mongoResource)
        {
            _mongoResource = mongoResource;
        }

        [Fact]
        public async Task GetByScopesTest_AssertResult()
        {
            // Arrange
            IIdOpsDbContext context = CreateDbContext();

            await context.ApiResources.InsertManyAsync(TestData.GetApiResources());

            var store = new ApiResourceStore(context);

            // Act
            IReadOnlyList<Model.ApiResource> result =
                await store.GetByScopesAsync(
                    Guid.Parse("00000000-0001-0000-0000-000000000000"),
                    default);

            // Assert
            result.Count.Should().Be(2);
            result[0].Id.Should().Be(Guid.Parse("00000000-0001-0000-0000-000000000000"));
            result[1].Id.Should().Be(Guid.Parse("00000000-0002-0000-0000-000000000000"));
        }

        private IIdOpsDbContext CreateDbContext()
        {
            IMongoDatabase database = _mongoResource.CreateDatabase();
            return new IdOpsDbContext(new MongoOptions
            {
                ConnectionString = _mongoResource.ConnectionString,
                DatabaseName = database.DatabaseNamespace.DatabaseName
            });
        }
    }
}
