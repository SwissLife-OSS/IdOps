namespace IdOps.IdentityServer.Azure;

public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; } = default!;

    public int PrefetchCount { get; set; } = 10;
}

