using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class ClientTemplateType : ObjectType<ClientTemplate>
    {
        protected override void Configure(IObjectTypeDescriptor<ClientTemplate> descriptor)
        {
            descriptor.Field("secrets")
                .ResolveWith<Resolvers>(_ => _.GetSecretsAsync(default!, default!, default));

            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public async Task<IReadOnlyList<ClientTemplateSecret>> GetSecretsAsync(
                [Parent] ClientTemplate clientTemplate,
                [Service] IEnvironmentService service,
                CancellationToken cancellationToken)
            {
                List<ClientTemplateSecret> result = clientTemplate.Secrets.Select(s
                        => new ClientTemplateSecret
                        {
                            EnvironmentId = s.EnvironmentId,
                            Type = s.Type,
                            Value = (s.Value is { }) ? "***" : null
                        })
                    .ToList();

                IEnumerable<Environment>? envs =
                    await service.GetAllAsync(cancellationToken);

                foreach (Environment? env in envs)
                {
                    if (!result.Any(t => t.EnvironmentId == env.Id))
                    {
                        result.Add(new ClientTemplateSecret { EnvironmentId = env.Id });
                    }
                }

                return result;
            }

            public Task<Tenant> GetTenantAsync(
                [Parent] ClientTemplate template,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(template.Tenant, cancellationToken);
            }
        }
    }
}
