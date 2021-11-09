using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage.Mongo;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Environment = IdOps.Model.Environment;

namespace IdOps.Api
{
    public class IdOpsSeeder : IHostedService
    {
        private readonly User _seedUser = new("admin", "seeder", new[] { "IdOps.Admin" });

        private record EnvSeed(string EnvName, Guid EnvId, Guid ServerId, string ServerUrl);
        private readonly IReadOnlyList<EnvSeed> _environments = new[]
        {
            new EnvSeed("Development",
                Guid.Parse("21b37fad00c341948c20740f8561cea3"),
                Guid.Parse("5cde5ee9dac846ffb04e1079581c08a7"),
                "http://localhost:5001"),
            new EnvSeed("Staging",
                Guid.Parse("f0807e289ff441378cedc1f553697a4b"),
                Guid.Parse("495d19fbc9fc435382b1bfaf23081369"),
                "http://localhost:5002")
        };

        private readonly Guid _groupId = Guid.Parse("6fa2f06cd0f5498a84dec8605defaebd");
        private readonly string _group = "Contoso";
        private readonly ICollection<string> _tenants;
        private readonly string _grantType = "client_credentials";

        private readonly IUserContextAccessor _userContextAccessor;
        private readonly IIdOpsDbContext _idOpsDbContext;

        public IdOpsSeeder(
            IUserContextAccessor userContextAccessor,
            IIdOpsDbContext idOpsDbContext)
        {
            _userContextAccessor = userContextAccessor;
            _idOpsDbContext = idOpsDbContext;
            _tenants = new[] { "internal", "external" }
                .Select(tenant => $"{_group}-{tenant}").ToArray();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _userContextAccessor.Context = new DefaultUserContext(_seedUser, default!);

                Environment? checkIfDataIsSeeded = await _idOpsDbContext.Environments
                    .Find(x => x.Id == _environments.First().EnvId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (checkIfDataIsSeeded is not null)
                {
                    return;
                }
                await _idOpsDbContext.Environments.InsertManyAsync(
                    _environments.Select(e => new Environment { Id = e.EnvId, Name = e.EnvName }),
                    cancellationToken: cancellationToken);

                await _idOpsDbContext.Tenants.InsertManyAsync(
                    _tenants.Select(tenant => new Tenant
                    {
                        Color = "#14299c",
                        Description = $"{tenant} tenant",
                        Id = tenant,
                        RoleMappings = _environments
                            .Select(e => new TenantRoleMapping
                            {
                                EnvironmentId = e.EnvId,
                                ClaimValue = "IdOps.Admin",
                                Role = "IdOps.Admin"
                            }).ToArray()
                    }), cancellationToken: cancellationToken);

                await _idOpsDbContext.IdentityServerGroups.InsertOneAsync(
                    new IdentityServerGroup
                    {
                        Id = _groupId,
                        Color = "#14299c",
                        Name = _group,
                        Tenants = _tenants
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.IdentityServers.InsertManyAsync(
                    _environments.Select(e => new Model.IdentityServer
                    {
                        EnvironmentId = e.EnvId,
                        GroupId = _groupId,
                        Id = e.ServerId,
                        Name = $"{_group}-{e.EnvName}",
                        Url = e.ServerUrl,
                        Version = new ResourceVersion
                        {
                            CreatedAt = DateTime.UtcNow, CreatedBy = "seeder", Version = 1
                        }
                    }), cancellationToken: cancellationToken);

                await _idOpsDbContext.GrantTypes.InsertOneAsync(
                    new GrantType
                    {
                        Id = _grantType,
                        IsCustom = false,
                        Name = _grantType,
                        Tenants = _tenants,
                    }, cancellationToken: cancellationToken);

                await _idOpsDbContext.ClientTemplates.InsertManyAsync(
                    _tenants.Select(tenant => new ClientTemplate
                    {
                        Name = "BackendService",
                        Tenant = tenant,
                        ClientIdGenerator = "GUID",
                        NameTemplate = "{{toUpper environment}}_{{application}}",
                        UrlTemplate = "https://{{application}}.{{environment}}.local",
                    }), cancellationToken: cancellationToken);
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
