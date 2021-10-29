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
    public class ApiResourceType : ObjectType<ApiResource>
    {
        protected override void Configure(IObjectTypeDescriptor<ApiResource> descriptor)
        {
            descriptor.Field("tenantInfo")
             .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));

            descriptor.Field(x => x.Scopes)
                 .ResolveWith<Resolvers>(_ => _
                    .GetScopesAsync(default!, default!, default!));
        }

        private class Resolvers
        {
            public Task<Tenant> GetTenantAsync(
                [Parent] ApiResource apiResource,
                [DataLoader] TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(apiResource.Tenant, cancellationToken);
            }

            public async Task<IEnumerable<ApiScope>> GetScopesAsync(
                [Parent] ApiResource apiResource,
                [Service] IApiScopeService apiScopeService,
                CancellationToken cancellationToken)
            {
                if (apiResource.Scopes != null)
                {
                    return await apiScopeService.GetManyAsync(
                        apiResource.Scopes,
                        cancellationToken);
                }

                return Array.Empty<ApiScope>();
            }
        }
    }
}
