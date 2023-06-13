using System;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;

public class KeyvaultEncryptedValue : EncryptedValue
{
    public KeyvaultEncryptedValue(EncryptResult result)
    {
        EncryptResult = result;
    }
    
    public EncryptResult EncryptResult { get; }

    public override string Kind => nameof(KeyvaultEncryptedValue);


    public string KeyId => EncryptResult.KeyId;

    public string Value => Convert.ToBase64String(EncryptResult.Ciphertext);
}