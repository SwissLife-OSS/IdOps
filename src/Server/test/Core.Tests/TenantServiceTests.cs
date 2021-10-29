using System;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace IdOps.Core.Tests
{
    public class TenantServiceTests
    {
        [Fact]
        public async Task Save_WithValidRequest_IsSaved()
        {
            Tenant createdTenant = null;

            // Arrange
            Mock<ITenantStore>? mock = CreateTenantStoreMock((tenant) =>
            {
                createdTenant = tenant;
            });

            var service = new TenantService(mock.Object, Mock.Of<IUserContextAccessor>());

            var request = new SaveTenantRequest("1", "green", "Foo");

            // Act
            Tenant tenant = await service.SaveAsync(request, default);

            // Assert
            mock.Verify(m => m.SaveAsync(
               It.IsAny<Tenant>(),
               It.IsAny<CancellationToken>()), Times.Once);

            //createdTenant.MatchSnapshot("Foo");
        }


        private Mock<ITenantStore> CreateTenantStoreMock(Action<Tenant> callback)
        {
            var mock = new Mock<ITenantStore>(MockBehavior.Strict);

            mock.Setup(m => m.SaveAsync(
               It.IsAny<Tenant>(),
               It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tenant request, CancellationToken cancellationToken) =>
                {
                    return new Tenant
                    {
                        Id = request.Id,
                        Color = request.Color
                    };
                })
                .Callback((Tenant tenant, CancellationToken cancellationToken) =>
                {
                    callback?.Invoke(tenant);
                });

            return mock;
        }
    }
}
