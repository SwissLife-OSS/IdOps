using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;
using IdOps.Templates;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace IdOps.Core.Tests
{
    public class ClientTemplateServiceTests
    {
        [Fact]
        public async Task SaveClient_FromClientTemplate_IsSaved()
        {
            Guid returnedTemplateId = Guid.Empty;
            string clientIdName = "";
            ICollection<IClientIdGenerator> clientIdGeneratorList = new List<IClientIdGenerator>();
            AddSecretRequest secretRequest = new AddSecretRequest();

            // Arrange
            Mock<IClientTemplateStore> mockTemplateStore = CreateClientTemplateStoreMock((templateId) =>
            {
                returnedTemplateId = templateId;
            });

            Mock<IClientIdGenerator> mockClientIdGenerator = CreateClientIdGeneratorMock((name) =>
            {
                clientIdName = name;
            });
            clientIdGeneratorList.Add(mockClientIdGenerator.Object);

            Mock<ISecretService> mockSecretService = CreateCreateSecretServiceMock((request) =>
            {
                secretRequest = request;
            });

            var templateId = Guid.NewGuid();

            Model.Environment env = new Model.Environment
            {
                Id = Guid.NewGuid()
            };

            Application application = new Application
            {
                Id = Guid.NewGuid(),
                Tenant = "TenantTest"
            };

            var service = new ClientTemplateService(
                mockTemplateStore.Object,
                clientIdGeneratorList,
                Mock.Of<IEnvironmentService>(),
                Mock.Of<ITemplateRenderer>(),
                mockSecretService.Object,
                Mock.Of<IUserContextAccessor>());

            // Act
            (Client client, string? secret) result = await service.CreateClientAsync(templateId, env, application, default);

            // Assert
            templateId.Should().Be(returnedTemplateId);

            mockTemplateStore.Verify(m => m.GetByIdAsync(
               It.IsAny<Guid>(),
               It.IsAny<CancellationToken>()), Times.Once);

            mockClientIdGenerator.Verify(m => m.CreateClientId(), Times.Once);

            mockSecretService.Verify(m => m.CreateSecretAsync(
               It.IsAny<AddSecretRequest>()), Times.Once);

            result.client.Environments.Should().Contain(env.Id);

            application.MatchSnapshot(x => x.IgnoreField("**.Id"));
        }

        private Mock<IClientTemplateStore> CreateClientTemplateStoreMock(Action<Guid> callback)
        {
            var mock = new Mock<IClientTemplateStore>(MockBehavior.Strict);

            mock.Setup(f => f.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken cancellationToken) =>
                {
                    return new ClientTemplate
                    {
                        Id = id
                    };
                })
                .Callback((Guid id, CancellationToken cancellationToken) =>
                {
                    callback?.Invoke(id);
                });

            return mock;
        }

        private Mock<IClientIdGenerator> CreateClientIdGeneratorMock(Action<string> callback)
        {
            var mock = new Mock<IClientIdGenerator>(MockBehavior.Loose);

            mock.Setup(f => f.CreateClientId())
                .Returns(() =>
                {
                    return "Client_Id";
                })
                .Callback(() =>
                {
                    callback?.Invoke("Client_Id");
                });


            return mock;
        }

        private Mock<ISecretService> CreateCreateSecretServiceMock(Action<AddSecretRequest> callback)
        {
            var mock = new Mock<ISecretService>(MockBehavior.Strict);

            mock.Setup(f => f.CreateSecretAsync(
                It.IsAny<AddSecretRequest>()))
                .ReturnsAsync((AddSecretRequest request) =>
                {
                    return (new Secret
                    {
                        Value = request.Value
                    }, "");
                })
                .Callback((AddSecretRequest request) =>
                {
                    callback?.Invoke(request);
                });

            return mock;
        }
    }
}
