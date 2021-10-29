namespace IdOps
{
    public interface IClientIdGenerator
    {
        string Name { get; }
        string CreateClientId();
    }
}
