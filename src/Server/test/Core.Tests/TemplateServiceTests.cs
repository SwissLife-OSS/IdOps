using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;
using IdOps.Templates;
using Moq;
using Xunit;

namespace IdOps.Core.Tests
{
    public class TemplateServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task CreateClient_WithClientUrlTemplate_UrlMatchAsync()
        {
            // Arrange
            var template = new ClientTemplate
            {
                Id = Guid.NewGuid(),
                UrlTemplate = "https://{{environment}}.{{application}}.foo.local",
                SecretGenerator = "DEFAULT",
                AllowedGrantTypes = new List<string> { "GT_1" }
            };

            var aplication = new Application
            {
                Id = Guid.NewGuid(),
                Name = "Bar",
                AllowedGrantTypes = new List<string> { "GT_2"}
            };

            var environment = new Model.Environment
            {
                Id = Guid.NewGuid(),
                Name = "Dev"
            };

            var service = new ClientTemplateService(
                Mock.Of<IClientTemplateStore>(),
                new List<IClientIdGenerator>
                {
                    new GuidClientIdGenerator()
                },
                Mock.Of<IEnvironmentService>(),
                new TemplateRenderer(),
                Mock.Of<ISecretService>(),
                Mock.Of<IUserContextAccessor>());

            // Act
            (Client client, string? secret) result = await service.CreateClientAsync(
                template,
                environment,
                aplication,
                CancellationToken.None);

            // Assert
            result.client.ClientUri.Should().Be("https://dev.bar.foo.local");
        }
    }
}
