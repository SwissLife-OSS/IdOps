using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.Jwk;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;

namespace IdOps
{
    public class IdentityServerService : UserTenantService , IIdentityServerService
    {
        private readonly IIdentityServerStore _identityServerStore;
        private readonly IResourceManager<Model.IdentityServer> _resourceManager;

        public IdentityServerService(
            IIdentityServerStore identityServerStore,
            IResourceManager<Model.IdentityServer> resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(userContextAccessor)
        {
            _identityServerStore = identityServerStore;
            _resourceManager = resourceManager;
        }

        public Task<IEnumerable<Model.IdentityServer>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return _identityServerStore.GetAllAsync(cancellationToken);
        }

        public Task<IEnumerable<IdentityServerGroup>> GetAllGroupsAsync(
            CancellationToken cancellationToken)
        {
            return _identityServerStore.GetAllGroupsAsync(cancellationToken);
        }

        public async Task<IEnumerable<IdentityServerGroup>> GetGroupsByUserTenants(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants =await GetUserTenantsAsync(cancellationToken);
            IEnumerable<IdentityServerGroup>? allGroups = await GetAllGroupsAsync(cancellationToken);

            var byTenant = new List<IdentityServerGroup>();

            foreach (IdentityServerGroup? group in allGroups)
            {
                if (group.Tenants.Any(userTenants.Contains))
                {
                    byTenant.Add(group);
                }
            }

            return byTenant;
        }

        public async Task<Model.IdentityServer> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _identityServerStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Model.IdentityServer> SaveAsync(
            SaveIdentityServerRequest request,
            CancellationToken cancellationToken)
        {
            Model.IdentityServer resource = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            resource.Name = request.Name;
            resource.GroupId = request.GroupId;
            resource.EnvironmentId = request.EnvironmentId;
            resource.Url = request.Url;

            SaveResourceResult<Model.IdentityServer> result = await _resourceManager.SaveAsync(
                resource,
                cancellationToken);

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

            string json = await httpClient.GetStringAsync(".well-known/openid-configuration");

            return json;
        }

        public async Task<IdentityServerGroup?> GetGroupByTenantAsync(
            string tenant,
            CancellationToken cancellationToken)
        {
            return await _identityServerStore
                .GetGroupByTenantAsync(tenant, cancellationToken);
        }

        public async Task<IdentityServerGroup> GetGroupByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _identityServerStore.GetGroupByIdAsync(id, cancellationToken);
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
