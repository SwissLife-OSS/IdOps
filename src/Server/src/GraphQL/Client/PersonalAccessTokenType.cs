using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class PersonalAccessTokenType : ObjectType<PersonalAccessToken>
    {
        protected override void Configure(IObjectTypeDescriptor<PersonalAccessToken> descriptor)
        {
            descriptor.Field("dependencies")
                .ResolveWith<DependencyResolvers>(_ => _
                    .GetDependenciesAsync(default!, default!, default!));

            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));

            descriptor.Field("applications")
                .ResolveWith<Resolvers>(_ => _.GetApplicationsAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<IReadOnlyList<Application>> GetApplicationsAsync(
                [Parent] PersonalAccessToken client,
                [Service] IApplicationService applicationService,
                CancellationToken cancellationToken)
            {
                return applicationService
                    .GetByIdsAsync(client.AllowedApplicationIds, cancellationToken);
            }

            public Task<Tenant> GetTenantAsync(
                [Parent] PersonalAccessToken client,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(client.Tenant, cancellationToken);
            }
        }
    }
}
