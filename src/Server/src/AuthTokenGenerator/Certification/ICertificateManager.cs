using System.Security.Cryptography.X509Certificates;

namespace IdOps.Certification
{
    public interface ICertificateManager
    {
        void AddToStore(
            X509Certificate2 certificate,
            StoreLocation location = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My);

        X509Certificate2 CreateSelfSignedCertificate(string subject, string password);

        X509Certificate2? GetFromStore(
            string thumbprint,
            StoreLocation location = StoreLocation.CurrentUser);
    }
}
