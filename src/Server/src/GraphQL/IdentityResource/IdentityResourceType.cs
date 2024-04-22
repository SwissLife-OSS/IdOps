using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class IdentityResourceType : ObjectType<IdentityResource>
    {
        protected override void Configure(IObjectTypeDescriptor<IdentityResource> descriptor)
        {
            descriptor
                .Field("identityServerGroup")
                .ResolveWith<Resolvers>(_
                    => _.GetIdentityServerGroup(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<IdentityServerGroup> GetIdentityServerGroup(
                [Parent] IdentityResource resource,
                IdentityServerGroupByIdDataLoader identityServerGroupById,
                CancellationToken cancellationToken)
            {
                return identityServerGroupById.LoadAsync(
                    resource.IdentityServerGroupId,
                    cancellationToken);
            }
        }
    }
}
