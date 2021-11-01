using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using FluentAssertions;
using IdOps.IdentityServer.Storage.Mongo.Tests;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Storage.Mongo.Tests
{
    [Collection(TestCollectionNames.Store)]
    public class PersistedGrantRepositoryTests : RepositoryTest
    {
        public PersistedGrantRepositoryTests(MongoResource mongoResource)
            : base(mongoResource)
        { }

        [Fact]
        public async Task Save_GrantFound()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            PersistedGrant? grant = DefaultGrant;

            //Act
            await repo.SaveAsync(grant, default);

            //Assert
            PersistedGrant? inserted = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            inserted.MatchSnapshot(o => o.IgnoreField("CreationTime"));
        }

        [Fact]
        public async Task Save_WhenExist_Updated()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            PersistedGrant grant = DefaultGrant;

            await dbContext.PersistedGrants.InsertOneAsync(grant, null, default);
            grant.SessionId = "UPDATED";

            //Act
            await repo.SaveAsync(grant, default);

            //Assert
            PersistedGrant? updated = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            updated.MatchSnapshot(o => o.IgnoreField("CreationTime"));
        }

        [Fact]
        public async Task Get_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            //Act
            PersistedGrant? grant = await repo.GetAsync(DefaultGrant.Key, default);

            //Assert
            grant.MatchSnapshot(o => o.IgnoreField("CreationTime"));
        }

        [Fact]
        public async Task GetByFilter_SubjectId_Found()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            var filter = new PersistedGrantFilter
            {
                SubjectId = DefaultGrant.SubjectId
            };

            //Act
            IEnumerable<PersistedGrant> grants = await repo.GetByFilterAsync(filter, default);

            //Assert
            grants.MatchSnapshot(o => o.IgnoreField("[*].CreationTime"));
        }

        [Fact]
        public async Task Delete_GrantDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            //Act
            await repo.DeleteAsync(DefaultGrant.Key, default);

            //Assert
            PersistedGrant? check = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            check.Should().BeNull();
        }

        [Fact]
        public async Task DeleteByFilter_SubjectId_GrantDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            var filter = new PersistedGrantFilter
            {
                SubjectId = DefaultGrant.SubjectId
            };

            //Act
            await repo.DeleteByFilterAsync(filter, default);

            //Assert
            PersistedGrant? check = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            check.Should().BeNull();
        }

        [Fact]
        public async Task DeleteByFilter_ClientId_GrantDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            var filter = new PersistedGrantFilter
            {
                ClientId = DefaultGrant.ClientId
            };

            //Act
            await repo.DeleteByFilterAsync(filter, default);

            //Assert
            PersistedGrant? check = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            check.Should().BeNull();
        }

        [Fact]
        public async Task DeleteByFilter_SessionId_GrantDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            var filter = new PersistedGrantFilter
            {
                SessionId = DefaultGrant.SessionId
            };

            //Act
            await repo.DeleteByFilterAsync(filter, default);

            //Assert
            PersistedGrant? check = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            check.Should().BeNull();
        }

        [Fact]
        public async Task DeleteByFilter_Type_GrantDeleted()
        {
            //Arrange
            IdentityStoreDbContext dbContext = CreateDbContext();
            PersistedGrantRepository repo = new PersistedGrantRepository(dbContext);

            await dbContext.PersistedGrants.InsertOneAsync(DefaultGrant, null, default);

            var filter = new PersistedGrantFilter
            {
                Type = DefaultGrant.Type
            };

            //Act
            await repo.DeleteByFilterAsync(filter, default);

            //Assert
            PersistedGrant? check = await GetGrantBykeyAsync(dbContext, DefaultGrant.Key);

            check.Should().BeNull();
        }

        private async Task<PersistedGrant> GetGrantBykeyAsync(
            IdentityStoreDbContext dbContext,
            string key)
        {
            return await dbContext.PersistedGrants.AsQueryable()
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync();
        }

        private PersistedGrant DefaultGrant => new PersistedGrant
        {
            ClientId = "123",
            SubjectId = "subject",
            Key = "ABC",
            SessionId = "Session",
            Type = "Z",
            CreationTime = DateTime.UtcNow
        };
    }
}
