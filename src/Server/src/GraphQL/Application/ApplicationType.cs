using System;
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
    public class ApplicationType : ObjectType<Application>
    {
        protected override void Configure(IObjectTypeDescriptor<Application> descriptor)
        {
            descriptor
                .Field("clients")
                .ResolveWith<Resolvers>(_ => _.GetClientsAsync(default!, default!, default!));

            descriptor
                .Field("userClaimRules")
                .ResolveWith<Resolvers>(_
                    => _.GetUserClaimRulesAsync(default!, default!, default!));

            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));

            descriptor.Field("template")
                .ResolveWith<Resolvers>(_ => _.GetTemplateAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public async Task<IEnumerable<Client>> GetClientsAsync(
                [Parent] Application application,
                [Service] IClientService clientService,
                CancellationToken cancellationToken)
            {
                return await clientService
                    .GetByIdsAsync(application.ClientIds, cancellationToken);
            }

            public Task<Tenant> GetTenantAsync(
                [Parent] Application application,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(application.Tenant, cancellationToken);
            }

            public Task<ClientTemplate> GetTemplateAsync(
                [Parent] Application application,
                ClientTemplateByIdDataLoader templateById,
                CancellationToken cancellationToken)
            {
                return templateById.LoadAsync(application.TemplateId, cancellationToken);
            }

            public Task<IReadOnlyList<UserClaimRule>> GetUserClaimRulesAsync(
                [Parent] Application application,
                [Service] IUserClaimRulesService userClaimRulesService,
                CancellationToken cancellationToken)
            {
                return userClaimRulesService
                    .GetByApplicationAsync(application.Id, cancellationToken);
            }
        }
    }
}
