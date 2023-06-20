using System;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;

public class KeyVaultEncryptedValue : EncryptedValue
{
    public KeyVaultEncryptedValue(EncryptResult result)
    {
        KeyId = result.KeyId;
        CipherText = result.Ciphertext;
    }

    public string KeyId { get; private set; }

    public byte[] CipherText { get; private set; }

    public override string Kind => nameof(KeyVaultEncryptedValue);

    public string Value => Convert.ToBase64String(CipherText);
}
