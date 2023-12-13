using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class ApiScopeType : ObjectType<ApiScope>
    {
        protected override void Configure(IObjectTypeDescriptor<ApiScope> descriptor)
        {
            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<Tenant> GetTenantAsync(
                [Parent] ApiScope apiScope,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(apiScope.Tenant, cancellationToken);
            }
        }
    }
}
