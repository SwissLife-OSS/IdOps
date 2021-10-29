using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer
{
    public interface IIdOpsIdentityServerBuilder
    {
        IConfiguration? Configuration { get; }
        IServiceCollection Services { get; }
        IdOpsOptions Options { get; }
    }

    public class IdOpsOptions
    {
        public string EnvironmentName { get; set; } = default!;

        public MessagingOptions Messaging { get; set; } = default!;

        public string ServerGroup { get; set; } = default!;

        public bool EnableDataConnectors { get; set; }
    }

    public class MessagingOptions
    {
        public MessagingTransport Transport { get; set; } = MessagingTransport.Memory;
    }

    public enum MessagingTransport
    {
        Memory,
        RabbitMq,
        AzureServiceBus
    }
}
