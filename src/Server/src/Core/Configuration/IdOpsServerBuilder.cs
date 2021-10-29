using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public class IdOpsServerBuilder : IIdOpsServerBuilder
    {
        public IdOpsServerBuilder(
            IConfiguration configuration,
            IServiceCollection services)
        {
            Configuration = configuration;
            Services = services;
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
    }
}
