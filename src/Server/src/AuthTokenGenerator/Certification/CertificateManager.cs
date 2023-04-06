using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdOps.Abstractions;
using IdOps.Certification;

namespace IdOps.Certifaction
{
    public class CertificateManager : ICertificateManager
    {
        public X509Certificate2 CreateSelfSignedCertificate(
            string subject,
            string password)
        {
            using var rsa = RSA.Create();
            var certRequest = new CertificateRequest($"cn={subject}",
                rsa, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            X509Certificate2 certificate = certRequest.CreateSelfSigned(
                DateTimeOffset.Now.AddMinutes(-5),
                DateTimeOffset.Now.AddYears(5));

            var exported = certificate.Export(X509ContentType.Pfx, password);

            return new X509Certificate2(exported, password, X509KeyStorageFlags.Exportable);
        }

        public void AddToStore(
            X509Certificate2 certificate,
            StoreLocation location = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My)
        {
            var store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);

            store.Close();
        }

        public X509Certificate2? GetFromStore(
            string thumbprint,
            StoreLocation location = StoreLocation.CurrentUser)
        {
            var store = new X509Store(location);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certs = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                thumbprint, false);

            if (certs.Count > 0)
            {
                return certs[0];
            }

            store.Close();

            return null;
        }
    }
}
