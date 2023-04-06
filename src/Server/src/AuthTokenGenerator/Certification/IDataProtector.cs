using IdOps.Models;

namespace IdOps.Certification
{
    public interface IDataProtector
    {
        string Name { get; }

        void Setup(EncryptionKeySetting settings);
        EncryptionKeySetting SetupNew();
        byte[] UnProtect(byte[] data);
        byte[] Protect(byte[] data);
    }
}
