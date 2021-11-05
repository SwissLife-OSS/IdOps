using System;

namespace IdOps.Model
{
    // TODO: Should it be IResource ?
    public class IdentityServer : IResource
    {
        public Guid Id { get; set; }

        public Guid EnvironmentId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Title => Name;

        public Guid GroupId { get; set; }

        public ResourceVersion Version { get; set; }

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Id == GroupId;
        }
    }
}
