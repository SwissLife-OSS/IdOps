using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class IdentityServerType : ObjectType<Model.IdentityServer>
    {
        protected override void Configure(IObjectTypeDescriptor<Model.IdentityServer> descriptor)
        {
            descriptor.Field("group")
                .ResolveWith<Resolvers>(_ => _.GetGroupAsync(default!, default!, default!));

            descriptor.Field("keys")
                .ResolveWith<Resolvers>(_ => _.GetKeysAsync(default!, default!, default!));

            descriptor.Field("discoveryDocument")
                .ResolveWith<Resolvers>(_ => _.GetDiscoveryDocumentAsync(
                    default!,
                    default!,
                    default!));
        }

        class Resolvers
        {
            public async Task<IdentityServerGroup> GetGroupAsync(
                [Parent] Model.IdentityServer server,
                IdentityServerGroupByIdDataLoader groupById,
                CancellationToken cancellationToken)
            {
                return await groupById.LoadAsync(server.GroupId, cancellationToken);
            }

            public async Task<IEnumerable<IdentityServerKey>> GetKeysAsync(
                [Parent] Model.IdentityServer server,
                [Service] IIdentityServerService identityServerService,
                CancellationToken cancellationToken)
            {
                return await identityServerService.GetKeysAsync(
                    server.Id,
                    cancellationToken);
            }

            public async Task<string> GetDiscoveryDocumentAsync(
                [Parent] Model.IdentityServer server,
                [Service] IIdentityServerService identityServerService,
                CancellationToken cancellationToken)
            {
                return await identityServerService.GetDiscoveryDocumentAsync(
                    server.Id,
                    cancellationToken);
            }
        }
    }
}
