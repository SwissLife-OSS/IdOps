using IdOps.Model;
using IdOps.Store.Mongo.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo
{
    public class IdOpsDbContext : MongoDbContext, IIdOpsDbContext
    {
        public IdOpsDbContext(MongoOptions mongoOptions)
            : base(mongoOptions)
        {
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
                .ConfigureCollection(new IdentityServerCollectionConfiguration())
                .ConfigureCollection(new IdentityServerGroupCollectionConfiguration())
                .ConfigureCollection(new SecureSecretCollectionConfiguration())
                .ConfigureCollection(new ClientTemplateCollectionConfiguration())
                .ConfigureCollection(new UserClaimRuleCollectionConfiguration())
                .ConfigureCollection(new PersonalAccessTokenCollectionConfiguration())
                .ConfigureCollection(new ApiResourceCollectionConfiguration());
        }

        public IMongoCollection<ApiResource> ApiResources
            => CreateCollection<ApiResource>();

        public IMongoCollection<ApiScope> ApiScopes
            => CreateCollection<ApiScope>();

        public IMongoCollection<IdentityResource> IdentityResources
            => CreateCollection<IdentityResource>();

        public IMongoCollection<Client> Clients
            => CreateCollection<Client>();

        public IMongoCollection<Application> Applications
            => CreateCollection<Application>();

        public IMongoCollection<Tenant> Tenants
            => CreateCollection<Tenant>();

        public IMongoCollection<Environment> Environments
            => CreateCollection<Environment>();

        public IMongoCollection<GrantType> GrantTypes
            => CreateCollection<GrantType>();

        public IMongoCollection<ResourceAuditEvent> ResouceAudits
            => CreateCollection<ResourceAuditEvent>();

        public IMongoCollection<ResourcePublishState> ResourcePublishStates
            => CreateCollection<ResourcePublishState>();

        public IMongoCollection<ResourcePublishLog> ResourcePublishLogs
            => CreateCollection<ResourcePublishLog>();

        public IMongoCollection<ResourceApprovalState> ResourceApprovalStates
            => CreateCollection<ResourceApprovalState>();

        public IMongoCollection<ResourceApprovalLog> ResourceApprovalLogs
            => CreateCollection<ResourceApprovalLog>();

        public IMongoCollection<IdentityServerEvent> IdentityServerEvents
            => CreateCollection<IdentityServerEvent>();

        public IMongoCollection<Model.IdentityServer> IdentityServers
            => CreateCollection<Model.IdentityServer>();

        public IMongoCollection<IdentityServerGroup> IdentityServerGroups
            => CreateCollection<IdentityServerGroup>();

        public IMongoCollection<SecureSecret> SecureSecrets
            => CreateCollection<SecureSecret>();

        public IMongoCollection<ClientTemplate> ClientTemplates
            => CreateCollection<ClientTemplate>();

        public IMongoCollection<UserClaimRule> UserClaimRules
            => CreateCollection<UserClaimRule>();

        public IMongoCollection<PersonalAccessToken> PersonalAccessTokens
            => CreateCollection<PersonalAccessToken>();
    }
}
