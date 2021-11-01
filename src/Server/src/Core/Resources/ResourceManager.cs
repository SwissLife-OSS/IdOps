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
    public class ResourceManager<T> : IResourceManager<T> where T : class, IResource, new()
    {
        private readonly IUserContextAccessor _userContextAccessor;
        private readonly IResouceAuditStore _resouceAuditStore;

        public ResourceManager(
            IUserContextAccessor userContextAccessor,
            IResouceAuditStore resouceAuditStore,
            IResourceStore<T> store)
        {
            Store = store;
            _userContextAccessor = userContextAccessor;
            _resouceAuditStore = resouceAuditStore;
        }
        public IResource? Original { get; private set; }

        public IResourceStore<T>? Store { get; }

        public IUserContext UserContext
        {
            get
            {
                return _userContextAccessor.Context ??
                       throw CouldNotAccessUserContextException.New();
            }
        }

        public T CreateNew()
        {
            var resource = new T();
            resource.Version = ResourceVersion.CreateNew(UserContext.UserId);
            resource.Id = Guid.NewGuid();

            return resource;
        }

        public void SetNewVersion(T resource)
        {
            // TODO: Why was createNew here?
            resource.Version = ResourceVersion.NewVersion(resource.Version, UserContext.UserId);
        }

        public async Task<T> GetExistingOrCreateNewAsync(
        Guid? id,
        CancellationToken cancellationToken)
        {
            IResource resource;

            if (id.HasValue)
            {
                resource = await Store!.GetByIdAsync(id.Value, cancellationToken);
                Original = resource.Clone();
            }
            else
            {
                resource = CreateNew();
            }

            return (T)resource;
        }

        public async Task<SaveResourceResult<T>> SaveAsync(
            T resource,
            CancellationToken cancellationToken)
        {
            ICollection<Difference>? diff = null;
            SaveResourceAction action = SaveResourceAction.Inserted;

            if (Original is { })
            {
                diff = Original.Diff(resource);

                if (diff.Any())
                {
                    resource.Version = ResourceVersion.NewVersion(
                        resource.Version,
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
                await Store!.SaveAsync(resource, cancellationToken);

                var audit = new ResourceAuditEvent
                {
                    Id = Guid.NewGuid(),
                    ResourceId = resource.Id,
                    Version = resource.Version.Version,
                    ResourceType = resource.GetType().Name,
                    UserId = UserContext.UserId,
                    Timestamp = DateTime.UtcNow,
                    Action = action.ToString(),
                    Changes = CreateChanges(diff)
                };

                await _resouceAuditStore.CreateAsync(audit, cancellationToken);
            }

            return new SaveResourceResult<T>(resource, action)
            {
                Diff = diff
            };
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
