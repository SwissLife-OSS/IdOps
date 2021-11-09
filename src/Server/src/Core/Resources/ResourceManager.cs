using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnyClone;
using AnyDiff;
using AnyDiff.Extensions;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class ResourceManager : IResourceManager
    {
        private readonly IUserContextAccessor _userContextAccessor;
        private readonly IResouceAuditStore _resourceAuditStore;
        private readonly IPublishedResourceDependencyResolver _dependencyResolver;

        private readonly IDictionary<string, IResourceStore> _stores;

        public ResourceManager(
            IUserContextAccessor userContextAccessor,
            IResouceAuditStore resourceAuditStore,
            IEnumerable<IResourceStore> stores,
            IPublishedResourceDependencyResolver dependencyResolver)
        {
            _stores = stores.ToDictionary(x => x.Type);
            _userContextAccessor = userContextAccessor;
            _resourceAuditStore = resourceAuditStore;
            _dependencyResolver = dependencyResolver;
        }

        private IUserContext UserContext =>
            _userContextAccessor.Context ?? throw CouldNotAccessUserContextException.New();

        public ResourceChangeContext<T> CreateNew<T>() where T : IResource, new()
        {
            var resource = new T();
            resource.Version = ResourceVersion.CreateNew(UserContext.UserId);
            resource.Id = Guid.NewGuid();

            return ResourceChangeContext<T>.FromNew(resource);
        }

        public ResourceChangeContext<T> SetNewVersion<T>(T resource)
            where T : class, IResource, new()
        {
            // TODO: Why was createNew here?
            resource.Version = ResourceVersion.NewVersion(resource.Version, UserContext.UserId);
            // TODO: Should we clone here also?
            return ResourceChangeContext<T>.FromNew(resource);
        }

        public async Task<ResourceChangeContext<T>> GetExistingOrCreateNewAsync<T>(
            Guid? id,
            CancellationToken cancellationToken)
            where T : class, IResource, new()
        {
            T? resource, original = default;

            if (id.HasValue)
            {
                resource = await GetStore<T>().GetByIdAsync(id.Value, cancellationToken);
                original = resource.Clone();
            }
            else
            {
                resource = CreateNew<T>().Resource;
            }

            return new ResourceChangeContext<T>(resource, original);
        }

        public async Task<SaveResourceResult<T>> SaveAsync<T>(
            ResourceChangeContext<T> context,
            CancellationToken cancellationToken)
            where T : class, IResource, new()
        {
            SaveResourceAction action = SaveResourceAction.Inserted;

            if (context.HasExistingResource)
            {
                if (context.Diff.Any())
                {
                    context.Resource.Version = ResourceVersion.NewVersion(
                        context.Resource.Version,
                        UserContext.UserId);
                    action = SaveResourceAction.Updated;
                }
                else
                {
                    action = SaveResourceAction.UnChanged;
                }
            }

            if (action != SaveResourceAction.UnChanged)
            {
                await GetStore<T>().SaveAsync(context.Resource, cancellationToken);

                var audit = new ResourceAuditEvent
                {
                    Id = Guid.NewGuid(),
                    ResourceId = context.Resource.Id,
                    Version = context.Resource.Version.Version,
                    ResourceType = context.Resource.GetType().Name,
                    UserId = UserContext.UserId,
                    Timestamp = DateTime.UtcNow,
                    Action = action.ToString(),
                    Changes = CreateChanges(context.Diff)
                };

                await _resourceAuditStore.CreateAsync(audit, cancellationToken);
            }

            await InvalidateDependencies(context.Resource, typeof(T).Name, cancellationToken);

            return new SaveResourceResult<T>(context.Resource, action)
            {
                Diff = context.Diff
            };
        }

        private async Task InvalidateDependencies(
            IResource parentResource,
            string resourceType,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<IResource> dependencies = await _dependencyResolver
                .ResolveDependenciesAsync(parentResource.Id, resourceType, cancellationToken);

            foreach (IResource resource in dependencies)
            {
                resource.Version = ResourceVersion
                    .NewVersion(resource.Version, UserContext.UserId);

                IResourceStore store = GetStore(resource.GetType().Name);
                await store!.SaveAsync(resource, cancellationToken);

                var audit = new ResourceAuditEvent
                {
                    Id = Guid.NewGuid(),
                    ResourceId = resource.Id,
                    Version = resource.Version.Version,
                    ResourceType = resource.GetType().Name,
                    UserId = UserContext.UserId,
                    Timestamp = DateTime.UtcNow,
                    Action = SaveResourceAction.Updated.ToString()
                };

                await _resourceAuditStore.CreateAsync(audit, cancellationToken);

                await InvalidateDependencies(resource, resource.GetType().Name, cancellationToken);
            }
        }

        private IResourceStore<T> GetStore<T>() where T : class, IResource, new() =>
            (IResourceStore<T>)GetStore(typeof(T).Name);

        private IResourceStore GetStore(string resourceType)
        {
            if (_stores.TryGetValue(resourceType, out IResourceStore? store))
            {
                return store;
            }

            throw new InvalidOperationException($"Unknown resource store for type {resourceType}");
        }

        private IEnumerable<ResourceChange> CreateChanges(ICollection<Difference>? diffs)
        {
            var changes = new List<ResourceChange>();

            if (diffs is { })
            {
                foreach (Difference diff in diffs)
                {
                    changes.Add(new ResourceChange
                    {
                        Property = diff.Property,
                        Path = diff.Path,
                        Before = diff.LeftValue?.ToString(),
                        After = diff.RightValue?.ToString(),
                        Delta = diff.Delta?.ToString(),
                        ArrayIndex = diff.ArrayIndex
                    });
                }
            }

            return changes;
        }
    }

    public record SaveResourceResult<T>(T Resource, SaveResourceAction Action)
        where T : IResource, new()
    {
        public ICollection<Difference>? Diff { get; init; }
    }
}
