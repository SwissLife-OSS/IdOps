using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Services;
using IdOps.IdentityServer.Store;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace IdOps
{
    public class UserClaimsRulesServiceTests
    {
        [Fact]
        public async Task GetRulesByClient_ByTenantAndClientId_ReturnsSameTenantAndClientIdAndWithNullClient()
        {
            // Arrange
            Mock<IUserClaimRuleRepository> mock = CreateRepositoryMock();

            var service = new UserClaimsRulesService(mock.Object, new MemoryCache(new MemoryCacheOptions()));
            var client = new IdOpsClient
            {
                Tenant = "Tenant_A",
                ClientId = "client_a"
            };

            var claimType = new List<string>();

            // Act
            IReadOnlyList<UserClaimRule> rules = await service.GetRulesByClientAsync(client, claimType, CancellationToken.None);

            // Assert
            rules.MatchSnapshot();
        }

        private Mock<IUserClaimRuleRepository> CreateRepositoryMock()
        {
            var mock = new Mock<IUserClaimRuleRepository>();

            mock.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserClaimRule>
                {
                    new UserClaimRule
                    {
                        Name = "a",
                        Tenant = "Tenant_A",
                        ClientIds = new[] {"client_a" },
                    },
                    new UserClaimRule
                    {
                        Name = "a",
                        Tenant = "Tenant_A",
                        ClientIds = new[] {"client_z" },
                    },
                    new UserClaimRule
                    {
                        Name = "b",
                        Tenant = "Tenant_A",
                        ClientIds = Array.Empty<string>()
                    },
                    new UserClaimRule
                    {
                        Name = "c",
                        Tenant = "Tenant_B"
                    },
                    new UserClaimRule
                    {
                        Name = "d",
                        Tenant = "Tenant_B",
                        ClientIds = new[] {"client_d" },
                    }
                });

            return mock;
        }
    }
}
