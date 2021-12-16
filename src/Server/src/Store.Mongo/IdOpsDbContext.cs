using IdOps.Model;
using IdOps.Server.Storage.Mongo.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo
{
    public class IdOpsDbContext : MongoDbContext, IIdOpsDbContext
    {
        public IdOpsDbContext(MongoOptions mongoOptions)
            : base(mongoOptions)
        {
            ApiResources = CreateCollection<ApiResource>();
            ApiScopes = CreateCollection<ApiScope>();
            IdentityResources = CreateCollection<IdentityResource>();
            Clients = CreateCollection<Client>();
            Applications = CreateCollection<Application>();
            Tenants = CreateCollection<Tenant>();
            Environments = CreateCollection<Environment>();
            GrantTypes = CreateCollection<GrantType>();
            ResouceAudits = CreateCollection<ResourceAuditEvent>();
            ResourcePublishStates = CreateCollection<ResourcePublishState>();
            ResourcePublishLogs = CreateCollection<ResourcePublishLog>();
            ResourceApprovalStates = CreateCollection<ResourceApprovalState>();
            ResourceApprovalLogs = CreateCollection<ResourceApprovalLog>();
            IdentityServerEvents = CreateCollection<IdentityServerEvent>();
            IdentityServers = CreateCollection<Model.IdentityServer>();
            IdentityServerGroups = CreateCollection<IdentityServerGroup>();
            SecureSecrets = CreateCollection<SecureSecret>();
            ClientTemplates = CreateCollection<ClientTemplate>();
            UserClaimRules = CreateCollection<UserClaimRule>();
            PersonalAccessTokens = CreateCollection<PersonalAccessToken>();
        }

        protected override void OnConfiguring(IMongoDatabaseBuilder builder)
        {
            builder
                .RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String))
                .ConfigureConnection(con => con.ReadConcern = ReadConcern.Majority)
                .ConfigureConnection(con => con.WriteConcern = WriteConcern.WMajority)
                .ConfigureConnection(con => con.ReadPreference = ReadPreference.Primary)
                .ConfigureCollection(new ApiScopeCollectionConfiguration())
                .ConfigureCollection(new IdentityResourceCollectionConfiguration())
                .ConfigureCollection(new ClientCollectionConfiguration())
                .ConfigureCollection(new ApplicationCollectionConfiguration())
                .ConfigureCollection(new TenantCollectionConfiguration())
                .ConfigureCollection(new EnvironmentCollectionConfiguration())
                .ConfigureCollection(new ResourceAuditEventCollectionConfiguration())
                .ConfigureCollection(new GrantTypeCollectionConfiguration())
                .ConfigureCollection(new IdentityServerEventCollectionConfiguration())
                .ConfigureCollection(new ResourcePublishLogCollectionConfiguration())
                .ConfigureCollection(new ResourcePublishStateCollectionConfiguration())
                .ConfigureCollection(new ResourceApprovalLogCollectionConfiguration())
                .ConfigureCollection(new ResourceApprovalStateCollectionConfiguration())
                .ConfigureCollection(new IdentityServerCollectionConfiguration())
                .ConfigureCollection(new IdentityServerGroupCollectionConfiguration())
                .ConfigureCollection(new SecureSecretCollectionConfiguration())
                .ConfigureCollection(new ClientTemplateCollectionConfiguration())
                .ConfigureCollection(new UserClaimRuleCollectionConfiguration())
                .ConfigureCollection(new PersonalAccessTokenCollectionConfiguration())
                .ConfigureCollection(new ApiResourceCollectionConfiguration());
        }

        public IMongoCollection<ApiResource> ApiResources { get; }
        public IMongoCollection<ApiScope> ApiScopes { get; }
        public IMongoCollection<IdentityResource> IdentityResources { get; }
        public IMongoCollection<Client> Clients { get; }
        public IMongoCollection<Application> Applications { get; }
        public IMongoCollection<Tenant> Tenants { get; }
        public IMongoCollection<Environment> Environments { get; }
        public IMongoCollection<GrantType> GrantTypes { get; }
        public IMongoCollection<ResourceAuditEvent> ResouceAudits { get; }
        public IMongoCollection<ResourcePublishState> ResourcePublishStates { get; }
        public IMongoCollection<ResourcePublishLog> ResourcePublishLogs { get; }
        public IMongoCollection<ResourceApprovalState> ResourceApprovalStates { get; }
        public IMongoCollection<ResourceApprovalLog> ResourceApprovalLogs { get; }
        public IMongoCollection<IdentityServerEvent> IdentityServerEvents { get; }
        public IMongoCollection<Model.IdentityServer> IdentityServers { get; }
        public IMongoCollection<IdentityServerGroup> IdentityServerGroups { get; }
        public IMongoCollection<SecureSecret> SecureSecrets { get; }
        public IMongoCollection<ClientTemplate> ClientTemplates { get; }
        public IMongoCollection<UserClaimRule> UserClaimRules { get; }
        public IMongoCollection<PersonalAccessToken> PersonalAccessTokens { get; }
    }
}
