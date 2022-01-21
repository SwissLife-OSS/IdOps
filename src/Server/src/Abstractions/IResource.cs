using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public interface IResource
    {
        public Guid Id { get; set; }

        public string Title { get; }

        public ResourceVersion Version { get; set; }

        public bool IsInServerGroup(IdentityServerGroup serverGroup);

        public ICollection<Guid> GetEnvironmentIds() => _defaultEnvironment;

        public bool HasEnvironments() => !ReferenceEquals(GetEnvironmentIds(), _defaultEnvironment);

        private static readonly ICollection<Guid> _defaultEnvironment = new List<Guid>();
    }

    public interface ITenantResource : IResource, IHasTenant
    {
    }

    public interface IHasTenant
    {
        public string Tenant { get; }
    }

    public interface IAllowedScopesResource : IResource
    {
        public IReadOnlyList<Guid> GetAllowedScopesIds();
    }
}
