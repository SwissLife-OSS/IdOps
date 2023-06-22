using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.Jwk;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class IdentityServerService : UserTenantService , IIdentityServerService
    {
        private readonly IIdentityServerStore _identityServerStore;
        private readonly IResourceManager _resourceManager;

        public IdentityServerService(
            IIdentityServerStore identityServerStore,
            IResourceManager resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(userContextAccessor)
        {
            _identityServerStore = identityServerStore;
            _resourceManager = resourceManager;
        }

        public Task<IReadOnlyList<Model.IdentityServer>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return _identityServerStore.GetAllAsync(cancellationToken);
        }

        public Task<Model.IdentityServer> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _identityServerStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Model.IdentityServer> SaveAsync(
            SaveIdentityServerRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Model.IdentityServer> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Model.IdentityServer>(request.Id, cancellationToken);

            context.Resource.Name = request.Name;
            context.Resource.GroupId = request.GroupId;
            context.Resource.EnvironmentId = request.EnvironmentId;
            context.Resource.Url = request.Url;

            SaveResourceResult<Model.IdentityServer> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<string> GetDiscoveryDocumentAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Model.IdentityServer? server = await _identityServerStore
                .GetByIdAsync(id, cancellationToken);

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(server.Url)
            };

            string json = await httpClient.GetStringAsync(
                ".well-known/openid-configuration", 
                cancellationToken);

            return json;
        }

        public async Task<IEnumerable<IdentityServerKey>> GetKeysAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Model.IdentityServer? server = await _identityServerStore
                .GetByIdAsync(id, cancellationToken);

            HttpClient httpClient = new();
            DiscoveryDocumentResponse disco = await httpClient.GetDiscoveryDocumentAsync(
                server.Url,
                cancellationToken);

            var keys = new List<IdentityServerKey>();

            foreach (JsonWebKey key in disco.KeySet.Keys)
            {
                if (key.Kty == "RSA")
                {
                    var cert = new X509Certificate2(
                        Convert.FromBase64String(key.X5c.First()));

                    var keyInfo = new IdentityServerKey(key.Kid, key.Alg ?? "RS256")
                    {
                        Thumbprint = cert.Thumbprint,
                        SerialNumber = cert.SerialNumber,
                        Subject = cert.Subject,
                        ValidUntil = cert.NotAfter
                    };
                    keys.Add(keyInfo);
                }
                else
                {
                    keys.Add(new IdentityServerKey(key.Kid, key.Alg));
                }
            }

            return keys;
        }
    }
}
