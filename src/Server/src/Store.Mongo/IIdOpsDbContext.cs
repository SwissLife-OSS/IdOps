using IdOps.Model;
using MongoDB.Driver;

namespace IdOps.Store.Mongo
{
    public interface IIdOpsDbContext
    {
        IMongoCollection<ApiResource> ApiResources { get; }
        IMongoCollection<ApiScope> ApiScopes { get; }
        IMongoCollection<IdentityResource> IdentityResources { get; }
        IMongoCollection<Client> Clients { get; }
        IMongoCollection<Application> Applications { get; }
        IMongoCollection<Tenant> Tenants { get; }
        IMongoCollection<Environment> Environments { get; }
        IMongoCollection<GrantType> GrantTypes { get; }
        IMongoCollection<ResourceAuditEvent> ResouceAudits { get; }
        IMongoCollection<ResourcePublishState> ResourcePublishStates { get; }
        IMongoCollection<ResourcePublishLog> ResourcePublishLogs { get; }
        IMongoCollection<ResourceApprovalState> ResourceApprovalStates { get; }
        IMongoCollection<ResourceApprovalLog> ResourceApprovalLogs { get; }
        IMongoCollection<IdentityServerEvent> IdentityServerEvents { get; }
        IMongoCollection<Model.IdentityServer> IdentityServers { get; }
        IMongoCollection<SecureSecret> SecureSecrets { get; }
        IMongoCollection<ClientTemplate> ClientTemplates { get; }
        IMongoCollection<IdentityServerGroup> IdentityServerGroups { get; }
        IMongoCollection<UserClaimRule> UserClaimRules { get; }
        IMongoCollection<PersonalAccessToken> PersonalAccessTokens { get; }
    }
}
