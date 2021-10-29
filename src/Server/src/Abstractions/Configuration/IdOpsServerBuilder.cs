using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public interface IIdOpsServerBuilder
    {
        IConfiguration Configuration { get; }

        IServiceCollection Services { get; }
    }
}
