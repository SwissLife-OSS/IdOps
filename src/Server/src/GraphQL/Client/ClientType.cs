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
    public class ClientType : ObjectType<Client>
    {
        protected override void Configure(IObjectTypeDescriptor<Client> descriptor)
        {
            descriptor.Field("identityScopes")
                .Resolve(x => x.Parent<Client>()
                    .AllowedScopes
                    .Where(s => s.Type == ScopeType.Identity)
                    .Select(s => s.Id));

            descriptor.Field("apiScopes")
                .Resolve(x => x.Parent<Client>()
                    .AllowedScopes
                    .Where(s => s.Type == ScopeType.Resource)
                    .Select(s => s.Id));

            descriptor.Field("dependencies")
                .ResolveWith<DependencyResolvers>(_ => _
                    .GetDependenciesAsync(default!, default!, default!));

            descriptor.Field("tenantInfo")
                .ResolveWith<Resolvers>(_ => _.GetTenantAsync(default!, default!, default!));

            descriptor.Field("application")
                .ResolveWith<Resolvers>(_ => _.GetApplicationAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public Task<Application?> GetApplicationAsync(
                [Parent] Client client,
                [Service] IApplicationService applicationService,
                CancellationToken cancellationToken)
            {
                //TODO: Add DataLoader
                return applicationService.GetByClientIdAsync(client.Id, cancellationToken);
            }

            public Task<Tenant> GetTenantAsync(
                [Parent] Client client,
                TenantByIdDataLoader tenantbyId,
                CancellationToken cancellationToken)
            {
                return tenantbyId.LoadAsync(client.Tenant, cancellationToken);
            }
        }
    }

    public class ResourceType : ObjectType<IResource>
    {
        protected override void Configure(IObjectTypeDescriptor<IResource> descriptor)
        {
            descriptor.Field("dependencies")
                .ResolveWith<DependencyResolvers>(_ => _
                    .GetDependenciesAsync(default!, default!, default!));
        }
    }

    internal class DependencyResolvers
    {
        public async Task<IEnumerable<IResource>> GetDependenciesAsync(
            IResource resource,
            [Service] IEnumerable<IResourceDependencyResolver> dependencyResolvers,
            CancellationToken cancellationToken)
        {
            List<IResource> dependencies = new List<IResource>();

            // Can be done with IAsyncEnumerable
            foreach (IResourceDependencyResolver resolver in dependencyResolvers)
            {
                dependencies.AddRange(await resolver
                    .ResolveDependenciesAsync(resource.Id, cancellationToken));
            }

            return dependencies;
        }
    }
}
