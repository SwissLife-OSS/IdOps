using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IdOps.Model;
using static IdOps.ResourceStates;

namespace IdOps
{
    internal class PublishedResourceMatcher
    {
        private readonly ILookup<Guid, ResourcePublishState> _states;
        private readonly IEnumerable<Model.Environment> _environments;
        private readonly IdentityServerGroup? _serverGroup;
        private readonly Dictionary<(Guid, Guid), ResourceApprovalEnvironment?> _approvalStates;

        internal PublishedResourceMatcher(
            IEnumerable<ResourcePublishState> states,
            IEnumerable<ResourceApproval> approvals,
            IEnumerable<Model.Environment> environments,
            IdentityServerGroup? serverGroup)
        {
            _states = states.ToLookup(x => x.ResourceId);
            _approvalStates = new Dictionary<(Guid, Guid), ResourceApprovalEnvironment?>();

            foreach (var approval in approvals)
            {
                foreach (var env in approval.Environments)
                {
                    _approvalStates[(approval.Id, env.EnvironmentId)] = env;
                }
            }

            _environments = environments;
            _serverGroup = serverGroup;
        }

        public bool TryMatchPublishedResources(
            IResourceService service,
            IResource resource,
            [NotNullWhen(true)] out PublishedResource? publishedResource)
        {
            publishedResource = null;

            IEnumerable<Model.Environment> environments = resource.HasEnvironments()
                ? _environments.Where(e => resource.GetEnvironmentIds().Contains(e.Id))
                : _environments;

            if (_serverGroup is { } && !resource.IsInServerGroup(_serverGroup))
            {
                return false;
            }

            publishedResource =
                new(resource.Id, resource.Title, service.ResourceType, resource.Version);

            foreach (Model.Environment? environment in environments)
            {
                ResourcePublishState? publishState =
                    _states[resource.Id].FirstOrDefault(x => x.EnvironmentId == environment.Id);

                var approvalState =
                    _approvalStates.GetValueOrDefault((resource.Id, environment.Id), null);

                int version = resource.Version.Version;
                string environmentState = (publishState, approvalState) switch
                {
                    (null, null) => NotDeployed,
                    (null, { }) when approvalState.Version == version => NotDeployed,
                    ({ }, _) when publishState.Version >= version => Latest,
                    ({ }, null) when publishState.Version < version => NotDeployed,
                    _ => NotApproved
                };

                PublishedResourceEnvironment publishedEnvironment = new(environment.Id)
                {
                    Version = publishState?.Version,
                    PublishedAt = publishState?.PublishedAt,
                    ApprovedAt = approvalState?.ApprovedAt,
                    State = environmentState
                };

                publishedResource.Environments.Add(publishedEnvironment);
            }

            return true;
        }
    }
}
