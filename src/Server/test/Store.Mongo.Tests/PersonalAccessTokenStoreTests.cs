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
    public class PersonalAccessTokenStoreTests : IClassFixture<MongoResource>
    {
        private readonly MongoResource _mongoResource;

        public PersonalAccessTokenStoreTests(MongoResource mongoResource)
        {
            _mongoResource = mongoResource;
        }

        [Fact]
        public async Task GetByAllowedScopesTest_AssertResult()
        {
            // Arrange
            IIdOpsDbContext context = CreateDbContext();

            await context.PersonalAccessTokens.InsertManyAsync(TestData.GetPersonalAccessTokens());

            var store = new PersonalAccessTokenStore(context);

            // Act
            IReadOnlyList<Model.PersonalAccessToken>? result =
                await store.GetByAllowedScopesAsync(
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
