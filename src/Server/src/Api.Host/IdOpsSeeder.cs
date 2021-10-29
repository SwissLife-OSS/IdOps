using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store.Mongo;
using Microsoft.Extensions.Hosting;

namespace IdOps.Api
{
    public class IdOpsSeeder : IHostedService
    {
        private readonly User _seedUser = new("admin", "seeder", new[] { "IdOps.Admin" });

        private readonly Dictionary<Guid, string> _environments = new()
        {
            { Guid.Parse("21b37fad00c341948c20740f8561cea3"), "Development" },
            { Guid.Parse("f0807e289ff441378cedc1f553697a4b"), "Staging" }
        };

        private readonly Guid _identityServerId = Guid.Parse("5cde5ee9dac846ffb04e1079581c08a7");
        private readonly Guid _groupId = Guid.Parse("6fa2f06cd0f5498a84dec8605defaebd");
        private readonly string _tenantName = "Contoso";
        private readonly string _grantType = "client_credentials";

        private readonly IUserContextAccessor _userContextAccessor;
        private readonly IIdOpsDbContext _idOpsDbContext;

        public IdOpsSeeder(
            IUserContextAccessor userContextAccessor,
            IIdOpsDbContext idOpsDbContext)
        {
            _userContextAccessor = userContextAccessor;
            _idOpsDbContext = idOpsDbContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _userContextAccessor.Context = new DefaultUserContext(_seedUser, default!);

                await _idOpsDbContext.Environments.InsertManyAsync(
                    _environments.Select(e => new Model.Environment { Id = e.Key, Name = e.Value }),
                    cancellationToken: cancellationToken);

                await _idOpsDbContext.Tenants.InsertOneAsync(
                    new Tenant
                    {
                        Color = "#14299c",
                        Description = "Contoso test tenant",
                        Id = _tenantName,
                        RoleMappings = _environments.Keys
                            .Select(k => new TenantRoleMapping
                            {
                                EnvironmentId = k,
                                ClaimValue = "IdOps.Admin",
                                Role = "IdOps.Admin"
                            }).ToArray()
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.IdentityServerGroups.InsertOneAsync(
                    new IdentityServerGroup
                    {
                        Id = _groupId,
                        Color = "#14299c",
                        Name = _tenantName,
                        Tenants = new[] { _tenantName }
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.IdentityServers.InsertOneAsync(
                    new Model.IdentityServer
                    {
                        EnvironmentId = _environments.Keys.ElementAt(0),
                        GroupId = _groupId,
                        Id = _identityServerId,
                        Name = _tenantName,
                        Url = "http://localhost:5001",
                        Version = new ResourceVersion
                        {
                            CreatedAt = DateTime.UtcNow, CreatedBy = "seeder", Version = 1
                        }
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.GrantTypes.InsertOneAsync(
                    new GrantType
                    {
                        Id = _grantType,
                        IsCustom = false,
                        Name = _grantType,
                        Tenants = new[] { _tenantName },
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.ClientTemplates.InsertOneAsync(
                    new ClientTemplate
                    {
                        Name = "BackendService",
                        Tenant = _tenantName,
                        ClientIdGenerator = "GUID",
                        NameTemplate = "{{toUpper environment}}_{{application}}",
                        UrlTemplate = "https://{{application}}.{{environment}}.foo.local",
                    }, cancellationToken: cancellationToken);
            }
            finally
            {
                _userContextAccessor.Context = default;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
