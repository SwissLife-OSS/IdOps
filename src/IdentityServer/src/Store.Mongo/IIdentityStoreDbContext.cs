using Duende.IdentityServer.Models;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;

namespace  IdOps.IdentityServer.Store.Mongo
{
    public interface IIdentityStoreDbContext
    {
        IMongoCollection<IdOpsClient> Clients { get; }
        IMongoCollection<PersistedGrant> PersistedGrants { get; }
        IMongoCollection<IdOpsApiScope> ApiScopes { get; }
        IMongoCollection<IdOpsIdentityResource> IdentityResources { get; }
        IMongoCollection<IdOpsApiResource> ApiResources { get; }
        IMongoCollection<UserClaimRule> UserClaimRules { get; }
        IMongoCollection<UserDataConnectorData> ConnectorData { get; }
        IMongoCollection<IdOpsPersonalAccessToken> PersonalAccessTokens { get; }
    }
}
