using System;

namespace IdOps.Model
{
    public class SecureSecret
    {
        public Guid Id { get; set; }

        public string CipherValue { get; set; }

        public SecretEncryptionInfo EncryptionInfo { get; set; }
    }
}
