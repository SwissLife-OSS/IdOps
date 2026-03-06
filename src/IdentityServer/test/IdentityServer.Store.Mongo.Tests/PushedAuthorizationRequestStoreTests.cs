using System;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Storage.Mongo.Tests
{
    [Collection(TestCollectionNames.Store)]
    public class PushedAuthorizationRequestStoreTests : RepositoryTest
    {
        public PushedAuthorizationRequestStoreTests(MongoResource mongoResource)
            : base(mongoResource)
        { }

        [Fact]
        public async Task Store_RequestFound()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            PushedAuthorizationRequest request = DefaultRequest;

            //Act
            await store.StoreAsync(request);

            //Assert
            PushedAuthorizationRequest? inserted = await GetRequestByHashAsync(
                dbContext, 
                DefaultRequest.ReferenceValueHash);

            inserted.Should().NotBeNull();
            inserted!.ReferenceValueHash.Should().Be(DefaultRequest.ReferenceValueHash);
            inserted.Parameters.Should().Be(DefaultRequest.Parameters);
        }

        [Fact]
        public async Task Store_WhenExists_Updated()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            PushedAuthorizationRequest request = DefaultRequest;

            await dbContext.PushedAuthorizationRequests.InsertOneAsync(request);
            
            var updatedRequest = new PushedAuthorizationRequest
            {
                ReferenceValueHash = request.ReferenceValueHash,
                ExpiresAtUtc = request.ExpiresAtUtc,
                Parameters = "updated_parameters"
            };

            //Act
            await store.StoreAsync(updatedRequest);

            //Assert
            PushedAuthorizationRequest? updated = await GetRequestByHashAsync(
                dbContext, 
                DefaultRequest.ReferenceValueHash);

            updated.Should().NotBeNull();
            updated!.Parameters.Should().Be("updated_parameters");
        }

        [Fact]
        public async Task GetByHash_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            await dbContext.PushedAuthorizationRequests.InsertOneAsync(DefaultRequest);

            //Act
            PushedAuthorizationRequest? result = await store.GetByHashAsync(
                DefaultRequest.ReferenceValueHash);

            //Assert
            result.Should().NotBeNull();
            result!.ReferenceValueHash.Should().Be(DefaultRequest.ReferenceValueHash);
            result.Parameters.Should().Be(DefaultRequest.Parameters);
        }

        [Fact]
        public async Task GetByHash_NotFound_ReturnsNull()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            //Act
            PushedAuthorizationRequest? result = await store.GetByHashAsync("non_existent_hash");

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ConsumeByHash_RequestDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            await dbContext.PushedAuthorizationRequests.InsertOneAsync(DefaultRequest);

            //Act
            await store.ConsumeByHashAsync(DefaultRequest.ReferenceValueHash);

            //Assert
            PushedAuthorizationRequest? check = await GetRequestByHashAsync(
                dbContext, 
                DefaultRequest.ReferenceValueHash);

            check.Should().BeNull();
        }

        [Fact]
        public async Task ConsumeByHash_NonExistent_NoError()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PushedAuthorizationRequestStore store = new PushedAuthorizationRequestStore(dbContext);

            //Act & Assert - should not throw
            await store.ConsumeByHashAsync("non_existent_hash");
        }

        private async Task<PushedAuthorizationRequest?> GetRequestByHashAsync(
            IdentityStoreDbContext dbContext,
            string hash)
        {
            return await dbContext.PushedAuthorizationRequests.AsQueryable()
                .Where(x => x.ReferenceValueHash == hash)
                .FirstOrDefaultAsync();
        }

        private PushedAuthorizationRequest DefaultRequest => new PushedAuthorizationRequest
        {
            ReferenceValueHash = "test_hash_123",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10),
            Parameters = "client_id=test&redirect_uri=https://example.com&scope=openid"
        };
    }
}
