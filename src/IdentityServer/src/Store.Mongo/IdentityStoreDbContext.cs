using Duende.IdentityServer.Models;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class IdentityStoreDbContext : MongoDbContext, IIdentityStoreDbContext
    {
        public IdentityStoreDbContext(IdOpsMongoOptions mongoOptions)
            : base(mongoOptions, enableAutoInitialize: false)
        {
            _collectionNames = mongoOptions.CollectionNames;
            Initialize();
        }

        private IMongoCollection<IdOpsClient>? _clients = null;
        private IMongoCollection<PersistedGrant>? _grants = null;
        private IMongoCollection<IdOpsApiScope>? _apiScopes = null;
        private IMongoCollection<IdOpsIdentityResource>? _identityResources = null;
        private IMongoCollection<IdOpsApiResource>? _apiResources = null;
        private IMongoCollection<UserClaimRule>? _userClaimRules = null;
        private IMongoCollection<UserDataConnectorData>? _connectorData = null;
        private IMongoCollection<IdOpsPersonalAccessToken>? _personalAccessTokens = null;

        private CollectionNames _collectionNames = new CollectionNames();

        public IMongoCollection<IdOpsClient> Clients
        {
            get
            {
                if (_clients == null)
                    _clients = CreateCollection<IdOpsClient>();
                return _clients;
            }
        }

        public IMongoCollection<PersistedGrant> PersistedGrants
        {
            get
            {
                if (_grants == null)
                    _grants = CreateCollection<PersistedGrant>();
                return _grants;
            }
        }

        public IMongoCollection<IdOpsApiScope> ApiScopes
        {
            get
            {
                if (_apiScopes == null)
                    _apiScopes = CreateCollection<IdOpsApiScope>();
                return _apiScopes;
            }
        }

        public IMongoCollection<IdOpsIdentityResource> IdentityResources
        {
            get
            {
                if (_identityResources == null)
                    _identityResources = CreateCollection<IdOpsIdentityResource>();
                return _identityResources;
            }
        }

        public IMongoCollection<IdOpsApiResource> ApiResources
        {
            get
            {
                if (_apiResources == null)
                    _apiResources = CreateCollection<IdOpsApiResource>();
                return _apiResources;
            }
        }

        public IMongoCollection<UserClaimRule> UserClaimRules
        {
            get
            {
                if (_userClaimRules == null)
                    _userClaimRules = CreateCollection<UserClaimRule>();
                return _userClaimRules;
            }
        }

        public IMongoCollection<UserDataConnectorData> ConnectorData
        {
            get
            {
                if (_connectorData == null)
                    _connectorData = CreateCollection<UserDataConnectorData>();
                return _connectorData;
            }
        }

        public IMongoCollection<IdOpsPersonalAccessToken> PersonalAccessTokens
        {
            get
            {
                if (_personalAccessTokens == null)
                    _personalAccessTokens = CreateCollection<IdOpsPersonalAccessToken>();
                return _personalAccessTokens;
            }
        }

        protected override void OnConfiguring(IMongoDatabaseBuilder mongoDatabaseBuilder)
        {
            mongoDatabaseBuilder
                .RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String))
                .RegisterConventionPack(
                    "enumRepresentation",
                    new ConventionPack {
                        new EnumRepresentationConvention(BsonType.String)},
                    t => true)
                .ConfigureConnection(con => con.ReadConcern = ReadConcern.Majority)
                .ConfigureConnection(con => con.WriteConcern = WriteConcern.WMajority)
                .ConfigureConnection(con => con.ReadPreference = ReadPreference.Primary)
                .ConfigureCollection(new ApiScopeCollectionConfiguration(_collectionNames.ApiScope))
                .ConfigureCollection(new PersistedGrantCollectionConfiguration())
                .ConfigureCollection(new IdentityResourceCollectionConfiguration(_collectionNames.IdentityResource))
                .ConfigureCollection(new ApiResourceCollectionConfiguration(_collectionNames.ApiResource))
                .ConfigureCollection(new UserClaimRuleCollectionConfiguration(_collectionNames.UserClaimRule))
                .ConfigureCollection(new UserDataConnectorDataCollectionConfiguration(_collectionNames.UserDataConnectorData))
                .ConfigureCollection(new IdOpsClientCollectionConfiguration(_collectionNames.Client))
                .ConfigureCollection(new PersonalAccessTokenCollectionConfiguration(_collectionNames.PersonalAccessTokens));
        }
    }
}
