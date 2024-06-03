namespace IdOps.IdentityServer.Azure;

public class EventHubOptions
{
    public string? ConnectionString { get; set; }

    public string? Namespace { get; set; }

    public EventStorageHubOptions? Storage { get; set; }
}
