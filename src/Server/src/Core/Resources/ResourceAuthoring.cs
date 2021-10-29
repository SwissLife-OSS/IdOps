using System;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public class ResourceAuthoring : IResourceAuthoring
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceAuthoring(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IClientService Clients
            => _serviceProvider.GetRequiredService<IClientService>();

        public IApplicationService Applications
            => _serviceProvider.GetRequiredService<IApplicationService>();

        public IEnvironmentService Environments
            => _serviceProvider.GetRequiredService<IEnvironmentService>();

        public ITenantService Tenants
            => _serviceProvider.GetRequiredService<ITenantService>();

        public IApiResourceService ApiResources
            => _serviceProvider.GetRequiredService<IApiResourceService>();
    }
}
