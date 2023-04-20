using IdOps.Models;

namespace IdOps.Certification
{
    public interface IDataProtector
    {
        string Name { get; }

        void Setup(EncryptionKeySetting settings);
        EncryptionKeySetting SetupNew();
        byte[] Unprotect(byte[] data);
        byte[] Protect(byte[] data);
    }
}
