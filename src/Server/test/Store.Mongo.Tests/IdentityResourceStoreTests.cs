using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.Model;
using IdOps.Server.Storage.Mongo;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;
using Xunit;

namespace IdOps.Server.Store.Mongo.Tests;

public class IdentityResourceStoreTests : IClassFixture<MongoResource>
{
    private readonly MongoResource _mongoResource;

    public IdentityResourceStoreTests(MongoResource mongoResource)
    {
        _mongoResource = mongoResource;
    }

    [Fact]
    public async Task GetAllTest_AssertResult()
    {
        // Arrange
        IIdOpsDbContext context = CreateDbContext();

        await context.IdentityResources.InsertManyAsync(TestData.GetIdentityResources());

        var store = new IdentityResourceStore(context);

        // Act
        IReadOnlyList<IdentityResource> result = await store.GetAllAsync(default, default, default);

        // Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(Guid.Parse("00000000-0001-0000-0000-000000000000"));
        result[1].Id.Should().Be(Guid.Parse("00000000-0002-0000-0000-000000000000"));
    }

    private IIdOpsDbContext CreateDbContext()
    {
        IMongoDatabase database = _mongoResource.CreateDatabase();

        var context = new IdOpsDbContext(
            new MongoOptions
            {
                ConnectionString = _mongoResource.ConnectionString,
                DatabaseName = database.DatabaseNamespace.DatabaseName
            },
            default);

        context.Initialize();

        return context;
    }
}
