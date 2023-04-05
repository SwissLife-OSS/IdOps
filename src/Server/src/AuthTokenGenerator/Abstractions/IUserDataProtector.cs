namespace IdOps.Abstractions
{
    public interface IUserDataProtector
    {
        byte[] UnProtect(byte[] data);
        byte[] Protect(byte[] data);
        string Protect(string value);
        string UnProtect(string value);
    }

    public record KeyContext(Guid Id, IDictionary<string, string> Parameters);

    public record ProtectedData(byte[] Data, Guid KeyId);
}
