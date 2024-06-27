using System.Collections.Generic;

namespace IdOps.Configuration
{
    public class IdOpsServerOptions
    {
        public MessagingOptions Messaging { get; set; } = default!;

        public HashSet<string> MutedClients { get; set; } = new HashSet<string>();

        public HashSet<string> NeedsApproval { get; set; } = new HashSet<string>();
    }

    public class MessagingOptions
    {
        public MessagingTransport Transport { get; set; } = MessagingTransport.Memory;

        public string ReceiverQueueName { get; set; } = "ops";

        public string Host { get; set; } = "localhost";

        public string Username { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public EventHubOptions? EventHub { get; set; } = default!;
    }

    public class EventHubOptions
    {
        public string? ConnectionString { get; set; }

        public string? Namespace { get; set; }

        public EventStorageHubOptions? Storage { get; set; }
    }

    public sealed class EventStorageHubOptions
    {
        public string? ConnectionString { get; set; }

        public string? Url { get; set; }
    }

    public enum MessagingTransport
    {
        Memory,
        RabbitMq,
        AzureServiceBus
    }
}
