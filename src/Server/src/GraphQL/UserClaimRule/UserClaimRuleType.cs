using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class UserClaimRuleType : ObjectType<UserClaimRule>
    {
        protected override void Configure(IObjectTypeDescriptor<UserClaimRule> descriptor)
        {
            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<Tenant> GetTenantAsync(
                [Parent] UserClaimRule rule,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(rule.Tenant, cancellationToken);
            }
        }
    }
}
