using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ClientCollectionConfiguration :
        IMongoCollectionConfiguration<Client>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Client> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Client)
                .AddBsonClassMap<Client>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
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
                    var clientIdIndex = new CreateIndexModel<Client>(
                         Builders<Client>.IndexKeys
                             .Ascending(c => c.ClientId),
                         new CreateIndexOptions { Unique = true });

                    var tenantIndex = new CreateIndexModel<Client>(
                        Builders<Client>.IndexKeys
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { clientIdIndex, tenantIndex });
                });
        }
    }
}
