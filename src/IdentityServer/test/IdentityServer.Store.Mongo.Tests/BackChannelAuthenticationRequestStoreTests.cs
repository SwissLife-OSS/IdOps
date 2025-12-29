using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using FluentAssertions;
using IdOps.IdentityServer.Storage.Mongo.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Storage.Mongo.Tests
{
    [Collection(TestCollectionNames.Store)]
    public class BackChannelAuthenticationRequestStoreTests : RepositoryTest
    {
        public BackChannelAuthenticationRequestStoreTests(MongoResource mongoResource)
            : base(mongoResource)
        { }

        [Fact]
        public async Task CreateRequest_RequestCreated()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();

            //Act
            string internalId = await store.CreateRequestAsync(request);

            //Assert
            internalId.Should().NotBeNullOrEmpty();
            
            SerializedBackChannelAuthenticationRequest? inserted = 
                await GetRequestByIdAsync(dbContext, internalId);

            inserted.Should().NotBeNull();
            inserted!.ClientId.Should().Be("test_client");
            inserted.SubjectClaims.Should().Contain(c => c.Type == "sub" && c.Value == "user123");
        }

        [Fact]
        public async Task CreateRequest_WithInternalId_UsesProvidedId()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();
            request.InternalId = "custom_internal_id";

            //Act
            string internalId = await store.CreateRequestAsync(request);

            //Assert
            internalId.Should().Be("custom_internal_id");
        }

        [Fact]
        public async Task GetByInternalId_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();
            string internalId = await store.CreateRequestAsync(request);

            //Act
            BackChannelAuthenticationRequest? result = await store.GetByInternalIdAsync(internalId);

            //Assert
            result.Should().NotBeNull();
            result!.ClientId.Should().Be("test_client");
            result.Subject.Should().NotBeNull();
            result.Subject!.FindFirst("sub")?.Value.Should().Be("user123");
        }

        [Fact]
        public async Task GetByInternalId_NotFound_ReturnsNull()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            //Act
            BackChannelAuthenticationRequest? result = 
                await store.GetByInternalIdAsync("non_existent_id");

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByAuthenticationRequestId_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();
            string internalId = await store.CreateRequestAsync(request);

            //Act
            BackChannelAuthenticationRequest? result = 
                await store.GetByAuthenticationRequestIdAsync(internalId);

            //Assert
            result.Should().NotBeNull();
            result!.ClientId.Should().Be("test_client");
        }

        [Fact]
        public async Task GetLoginsForUser_BySubjectId_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request1 = CreateDefaultRequest();
            BackChannelAuthenticationRequest request2 = CreateDefaultRequest();
            request2.ClientId = "another_client";

            await store.CreateRequestAsync(request1);
            await store.CreateRequestAsync(request2);

            //Act
            IEnumerable<BackChannelAuthenticationRequest> results = 
                await store.GetLoginsForUserAsync("user123");

            //Assert
            results.Should().HaveCount(2);
            results.Should().Contain(r => r.ClientId == "test_client");
            results.Should().Contain(r => r.ClientId == "another_client");
        }

        [Fact]
        public async Task GetLoginsForUser_BySubjectIdAndClientId_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request1 = CreateDefaultRequest();
            BackChannelAuthenticationRequest request2 = CreateDefaultRequest();
            request2.ClientId = "another_client";

            await store.CreateRequestAsync(request1);
            await store.CreateRequestAsync(request2);

            //Act
            IEnumerable<BackChannelAuthenticationRequest> results = 
                await store.GetLoginsForUserAsync("user123", "test_client");

            //Assert
            results.Should().HaveCount(1);
            results.First().ClientId.Should().Be("test_client");
        }

        [Fact]
        public async Task UpdateByInternalId_RequestUpdated()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();
            string internalId = await store.CreateRequestAsync(request);

            request.IsComplete = true;
            request.AuthorizedScopes = new[] { "openid", "profile" };

            //Act
            await store.UpdateByInternalIdAsync(internalId, request);

            //Assert
            BackChannelAuthenticationRequest? updated = 
                await store.GetByInternalIdAsync(internalId);

            updated.Should().NotBeNull();
            updated!.IsComplete.Should().BeTrue();
            updated.AuthorizedScopes.Should().Contain("openid");
            updated.AuthorizedScopes.Should().Contain("profile");
        }

        [Fact]
        public async Task RemoveByInternalId_RequestDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            BackChannelAuthenticationRequest request = CreateDefaultRequest();
            string internalId = await store.CreateRequestAsync(request);

            //Act
            await store.RemoveByInternalIdAsync(internalId);

            //Assert
            BackChannelAuthenticationRequest? check = 
                await store.GetByInternalIdAsync(internalId);

            check.Should().BeNull();
        }

        [Fact]
        public async Task RemoveByInternalId_NonExistent_NoError()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            //Act & Assert - should not throw
            await store.RemoveByInternalIdAsync("non_existent_id");
        }

        [Fact]
        public async Task ClaimsPrincipal_SerializedAndDeserialized_Correctly()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            BackChannelAuthenticationRequestStore store = 
                new BackChannelAuthenticationRequestStore(dbContext);

            var claims = new List<Claim>
            {
                new Claim("sub", "user123"),
                new Claim("name", "Test User"),
                new Claim("email", "test@example.com"),
                new Claim("role", "admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            BackChannelAuthenticationRequest request = new BackChannelAuthenticationRequest
            {
                ClientId = "test_client",
                Subject = principal,
                CreationTime = DateTime.UtcNow,
                Lifetime = 300,
                RequestedScopes = new[] { "openid" }
            };

            //Act
            string internalId = await store.CreateRequestAsync(request);
            BackChannelAuthenticationRequest? result = await store.GetByInternalIdAsync(internalId);

            //Assert
            result.Should().NotBeNull();
            result!.Subject.Should().NotBeNull();
            result.Subject!.Identity?.AuthenticationType.Should().Be("TestAuth");
            result.Subject.FindFirst("sub")?.Value.Should().Be("user123");
            result.Subject.FindFirst("name")?.Value.Should().Be("Test User");
            result.Subject.FindFirst("email")?.Value.Should().Be("test@example.com");
            result.Subject.FindFirst("role")?.Value.Should().Be("admin");
        }

        private async Task<SerializedBackChannelAuthenticationRequest?> GetRequestByIdAsync(
            IdentityStoreDbContext dbContext,
            string id)
        {
            return await dbContext.BackChannelAuthenticationRequests.AsQueryable()
                .Where(x => x.InternalId == id)
                .FirstOrDefaultAsync();
        }

        private BackChannelAuthenticationRequest CreateDefaultRequest()
        {
            var claims = new List<Claim>
            {
                new Claim("sub", "user123"),
                new Claim("name", "Test User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            return new BackChannelAuthenticationRequest
            {
                ClientId = "test_client",
                Subject = principal,
                CreationTime = DateTime.UtcNow,
                Lifetime = 300,
                RequestedScopes = new[] { "openid", "profile" },
                BindingMessage = "Please confirm login"
            };
        }
    }
}
