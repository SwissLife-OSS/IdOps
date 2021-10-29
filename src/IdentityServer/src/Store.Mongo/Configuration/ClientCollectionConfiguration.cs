using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace  IdOps.IdentityServer.Store.Mongo
{
    internal class IdOpsClientCollectionConfiguration
        : IMongoCollectionConfiguration<IdOpsClient>
    {
        private readonly string _collectionName;

        public IdOpsClientCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(IMongoCollectionBuilder<IdOpsClient> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<IdOpsClient>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.AutoMap();
                })
                .AddBsonClassMap<Client>( cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.ClientId);
                })
                .AddBsonClassMap<EnabledProvider>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var extIdIndex = new CreateIndexModel<IdOpsClient>(
                        Builders<IdOpsClient>.IndexKeys
                        .Ascending(c => c.ClientId)
                        .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });
                    collection.Indexes.CreateOne(extIdIndex);
                });
        }
    }
}
