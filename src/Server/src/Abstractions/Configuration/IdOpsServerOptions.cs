using System.Collections.Generic;

namespace IdOps.Configuration
{
    public class IdOpsServerOptions
    {
        public MessagingOptions Messaging { get; set; } = default!;

        public HashSet<string> MutedClients { get; set; } = new HashSet<string>();
    }

    public class MessagingOptions
    {
        public MessagingTransport Transport { get; set; } = MessagingTransport.Memory;

        public string ReceiverQueueName { get; set; } = "ops";

        public string Host { get; set; } = "localhost";

        public string Username { get; set; } = "guest";

        public string Password { get; set; } = "guest";
    }

    public enum MessagingTransport
    {
        Memory,
        RabbitMq,
        AzureServiceBus
    }
}
