using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;
using IdOps.Templates;
using Moq;

namespace IdOps.Core.Tests
{
    public class ServiceTestsBase
    {
        internal static ICollection<Guid> CreateIdentityScopes()
        {
            ICollection<Guid> identityScopes = new List<Guid>();
            identityScopes.Add(Guid.Parse("5135561c-6827-40c8-9f0f-3ffe8d1d9fbe"));
            return identityScopes;
        }

        internal static ICollection<Guid> CreateApiScopes()
        {
            ICollection<Guid> apiScopes = new List<Guid>();
            apiScopes.Add(Guid.Parse("202badbf-7922-4832-8651-ac7984d44774"));
            apiScopes.Add(Guid.Parse("5772ee6f-216c-4298-855c-42d64b946a40"));
            return apiScopes;
        }

        internal Mock<IClientService> CreateClientServiceMock()
        {
            var mock = new Mock<IClientService>(MockBehavior.Strict);
            mock.Setup(m => m.CreateClientAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Client());
            return mock;
        }

        internal Mock<IClientTemplateService> CreateClientTemplateServiceMock()
        {
            var mock = new Mock<IClientTemplateService>(MockBehavior.Strict);

            mock.Setup(t => t.CreateClientAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Model.Environment>(),
                    It.IsAny<Application>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    return (new Client { }, "fdasdfasdf");
                });

            return mock;
        }

        internal Mock<IResourceManager> CreateResourceManager_ApplicationMock()
        {
            var app = new Application { Id = Guid.Parse("aa69f6a7-e5bf-48c7-a6e2-3371d2503f53"), Name = "abc" };
            var expected = new SaveResourceResult<Application>(app, SaveResourceAction.Inserted);
            var mock = new Mock<IResourceManager>(MockBehavior.Strict);

            mock.Setup(m => m.CreateNew<Application>()).Returns(ResourceChangeContext<Application>.FromNew(app));

            mock.Setup(m => m.SaveAsync(It.IsAny<ResourceChangeContext<Application>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            return mock;
        }

        internal Mock<IUserContext> CreateUserContextMock(Action<string> callback)
        {
            var mock = new Mock<IUserContext>(MockBehavior.Strict);

            mock.SetupGet(m => m.UserId)
                .Returns("")
                .Callback(() =>
                {
                    callback?.Invoke("");
                });

            return mock;
        }

        internal Mock<IEnvironmentService> CreateEnvironmentServiceMock(
            IEnumerable<Model.Environment> environments,
            Action<IEnumerable<Model.Environment>> callback)
        {
            var mock = new Mock<IEnvironmentService>(MockBehavior.Strict);

            mock.Setup(f => f.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CancellationToken cancellationToken) =>
                {
                    return environments.ToArray();
                })
                .Callback((CancellationToken cancellationToken) =>
                {
                    callback?.Invoke(environments);
                });

            return mock;
        }

        internal Mock<ITenantStore> CreateTenantStoreMock(Action<Tenant> callback)
        {
            var mock = new Mock<ITenantStore>(MockBehavior.Strict);

            mock.Setup(m => m.SaveAsync(
                    It.IsAny<Tenant>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tenant request, CancellationToken cancellationToken) =>
                {
                    return new Tenant { Id = request.Id, Color = request.Color };
                })
                .Callback((Tenant tenant, CancellationToken cancellationToken) =>
                {
                    callback?.Invoke(tenant);
                });

            return mock;
        }
    }
}
