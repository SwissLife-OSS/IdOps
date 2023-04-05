using IdOps.Models;

namespace IdOps.Abstractions
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
