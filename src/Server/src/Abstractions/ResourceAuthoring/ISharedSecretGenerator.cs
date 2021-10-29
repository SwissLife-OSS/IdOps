namespace IdOps
{
    public interface ISharedSecretGenerator
    {
        string Name { get; }

        string CreateSecret();
    }
}
