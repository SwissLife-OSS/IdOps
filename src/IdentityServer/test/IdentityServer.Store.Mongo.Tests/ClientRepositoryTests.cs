using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.IdentityServer.Model;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Storage.Mongo.Tests
{
    [Collection(TestCollectionNames.Store)]
    public class ClientRepositoryTests : RepositoryTest
    {
        public ClientRepositoryTests(MongoResource mongoResource)
            : base(mongoResource)
        { }

        [Fact]
        public async Task GetAllClientOrigins_MatchSnapshot()
        {
            //Arrange
            ClientRepository repo = new ClientRepository(DbContext);

            await InsertClientAsync(new IdOpsClient
            {
                ClientId = "1",
                AllowedCorsOrigins = new List<string>
                {
                    "http://localhost",
                    "http://localhost:3000"
                }
            });

            await InsertClientAsync(new IdOpsClient
            {
                ClientId = "2",
                AllowedCorsOrigins = new List<string>
                {
                    "http://localhost:5000",
                }
            });

            //Act
            HashSet<string>? origins = await repo.GetAllClientOrigins();

            //Assert
            origins.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllClientRedirectUri_MatchSnapshot()
        {
            //Arrange
            ClientRepository repo = new ClientRepository(DbContext);

            await InsertClientAsync(new IdOpsClient
            {
                ClientId = "1",
                RedirectUris = new List<string>
                {
                    "http://localhost/callback",
                    "http://localhost:3000/callback"
                }
            });

            await InsertClientAsync(new IdOpsClient
            {
                ClientId = "2",
                RedirectUris = new List<string>
                {
                    "http://localhost:5000/callback",
                    "http://localhost/callback",
                }
            });

            //Act
            HashSet<string>? urls = await repo.GetAllClientRedirectUriAsync();

            //Assert
            urls.Should().HaveCount(3);
        }

        private async Task InsertClientAsync(IdOpsClient client)
        {
            await DbContext.Clients.InsertOneAsync(client, options: null, default);
        }
    }
}
