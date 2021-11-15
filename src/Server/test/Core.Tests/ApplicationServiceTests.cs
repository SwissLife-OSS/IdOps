using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.Model;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace IdOps.Core.Tests
{
    public class ApplicationServiceTests : ServiceTestsBase
    {
        [Fact]
        public async Task Save_WithValidRequest_IsSaved()
        {
            // Arrange
            Guid envId = Guid.Parse("e1579ce6-b8c3-4479-acf1-f35bbeee8b5b");

            IEnumerable<Model.Environment> environments =
                new List<Model.Environment>
                {
                    new Model.Environment { Id = envId, Name = "TestEnv", Order = 0 }
                };

            Mock<IResourceManager> resourceManagerMock = CreateResourceManager_ApplicationMock();

            Mock<IEnvironmentService> environmentServiceMock = CreateEnvironmentServiceMock(
                environments,
                (environmentList) =>
                {
                    environments = environmentList;
                });

            string tenant = "Contoso";

            ICollection<string> allowedGrantTypes = new List<string>();
            allowedGrantTypes.Add("asdf");

            ICollection<string> redirectUris = new List<string>();
            redirectUris.Add("https://asdf.ch");

            ICollection<Guid> env = new List<Guid>();
            env.Add(envId);

            var service = new ApplicationService(
                applicationStore: default,
                clientStore: default,
                userContextAccessor: default,
                resourceManager: resourceManagerMock.Object,
                clientService: CreateClientServiceMock().Object,
                clientTemplateService: CreateClientTemplateServiceMock().Object,
                environmentService: environmentServiceMock.Object);

            var request = new CreateApplicationRequest(
                Name: "MyApplication",
                Tenant: tenant,
                TemplateId: Guid.Parse("5bc2dd66f14245fca7a7e3b725f90147"),
                AllowedGrantTypes: allowedGrantTypes.ToArray(),
                ApiScopes: CreateApiScopes().ToArray(),
                IdentityScopes: CreateIdentityScopes().ToArray(),
                Environments: env.ToArray(),
                RedirectUris: redirectUris.ToArray());

            // Act
            ApplicationWithClients application = await service.CreateAsync(request, default);

            // Assert
            environmentServiceMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            application.Application.Tenant.Should().Be(tenant);
            application.Application.AllowedGrantTypes.Should().BeEquivalentTo(allowedGrantTypes);
            application.Application.RedirectUris.Should().BeEquivalentTo(redirectUris);


            ICollection<CreatedClientInfo> clients = (List<CreatedClientInfo>)application.Clients;
            CreatedClientInfo client = clients.FirstOrDefault();
            application.Application.ClientIds.Should().Contain(client.Id);

            application.MatchSnapshot(x => x.IgnoreField("**.Id").IgnoreField("**.ClientIds"));
        }
    }
}
