namespace IdOps.Server.Encryption.KeyVault;

internal class AzureKeyVaultOptions
{
    public string KeyVaultUri { get; set; }

    public string EncryptionKeyName { get; set; }
}
