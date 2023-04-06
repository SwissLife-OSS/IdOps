using IdOps.Models;

namespace IdOps.Certification
{
    public interface ISymmetricEncryption
    {
        byte[] DecryptFile(EncryptedDataEnvelope data, byte[] key);
        EncryptedDataEnvelope EncryptData(byte[] data, byte[] key);
    }
}
