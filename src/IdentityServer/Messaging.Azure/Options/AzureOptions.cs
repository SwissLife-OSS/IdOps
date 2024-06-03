namespace IdOps.IdentityServer.Azure;

public sealed class AzureOptions
{
    public AzureServiceBusOptions? ServiceBus { get; set; } = default!;
    public EventHubOptions? EventHub { get; set; } = default!;
}

