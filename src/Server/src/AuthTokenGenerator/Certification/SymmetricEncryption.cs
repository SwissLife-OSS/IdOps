using System.Security.Cryptography;
using IdOps.Abstractions;
using IdOps.Certification;

namespace IdOps.Models
{
    public record EncryptedDataEnvelope(byte[] Data, string Algorithm);

    public class SymmetricEncryption : ISymmetricEncryption
    {
        private readonly string Algorithm = "AES256";

        public EncryptedDataEnvelope EncryptData(byte[] data, byte[] key)
        {
            using var stream = new MemoryStream();
            using SymmetricAlgorithm symAlg = GetAlgorithm(Algorithm);

            symAlg.Key = key;
            symAlg.GenerateIV();

            using ICryptoTransform encryptionTransform = symAlg
                .CreateEncryptor(symAlg.Key, symAlg.IV);

            stream.Write(symAlg.IV, 0, symAlg.IV.Length);

            using var csEncrypt = new CryptoStream(
                stream, encryptionTransform, CryptoStreamMode.Write);

            csEncrypt.Write(data, 0, data.Length);
            csEncrypt.Close();

            return new EncryptedDataEnvelope(stream.ToArray(), "AES256");
        }

        public byte[] DecryptFile(EncryptedDataEnvelope data, byte[] key)
        {
            using var stream = new MemoryStream();
            using SymmetricAlgorithm symAlg = GetAlgorithm(Algorithm);

            int ivLength = (symAlg.BlockSize / 8);

            ReadOnlySpan<byte> securedDataSpan = data.Data;
            ReadOnlySpan<byte> ivSpan = securedDataSpan.Slice(0, ivLength);

            symAlg.IV = ivSpan.ToArray();
            symAlg.Key = key;

            using ICryptoTransform decryptionTransform =
                symAlg.CreateDecryptor(symAlg.Key, symAlg.IV);

            using var csDecrypt = new CryptoStream(
                stream, decryptionTransform, CryptoStreamMode.Write);

            csDecrypt.Write(data.Data, ivLength,
                (data.Data.Length - ivLength));
            csDecrypt.Close();

            byte[] plainData = stream.ToArray();

            return plainData;
        }

        private static SymmetricAlgorithm GetAlgorithm(string algorithm)
        {
            switch (algorithm.ToUpper())
            {
                case "AES256":
                    return new AesManaged()
                    {
                        KeySize = 256,
                        BlockSize = 128,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.PKCS7,
                    };
                default:
                    throw new ApplicationException(
                        $"The symmetric encryption algorithm '{algorithm}' " +
                        $"is not supported");
            }
        }
    }
}
