using System;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;

public class KeyvaultEncryptedValue : EncryptedValue
{
    public KeyvaultEncryptedValue(EncryptResult result)
    {
        KeyId = result.KeyId;
        CipherText = result.Ciphertext;
    }

    public string KeyId { get; private set; }
    
    public byte[] CipherText { get; private set; }
    
    public override string Kind => nameof(KeyvaultEncryptedValue);
    
    public string Value => Convert.ToBase64String(CipherText);
}