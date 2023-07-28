namespace IdOps;

public class Session
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string SecretId { get; set; }
    public string CodeVerifier { get; set; }
    public string SignalrConnectionId { get; set; }
    
    public string RedirectUrl { get; set; }
}
