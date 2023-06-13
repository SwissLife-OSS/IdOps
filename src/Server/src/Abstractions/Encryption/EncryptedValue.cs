namespace IdOps;

public abstract class EncryptedValue
{
    public abstract string Kind { get; } // keyvault, HMAC, AES, Whatever floats your boat
}
