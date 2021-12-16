using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ClientTemplateCollectionConfiguration :
        IMongoCollectionConfiguration<ClientTemplate>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ClientTemplate> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ClientTemplate)
                .AddBsonClassMap<ClientTemplate>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var uniquenessIndex = new CreateIndexModel<ClientTemplate>(
                        Builders<ClientTemplate>.IndexKeys
                            .Ascending(c => c.Name)
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });

                    var nameIndex = new CreateIndexModel<ClientTemplate>(
                        Builders<ClientTemplate>.IndexKeys
                            .Ascending(c => c.Name),
                        new CreateIndexOptions { Unique = false });

                    var tenantIndex = new CreateIndexModel<ClientTemplate>(
                        Builders<ClientTemplate>.IndexKeys
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });

                    collection.Indexes.CreateMany(new[] { uniquenessIndex, nameIndex, tenantIndex });
                });
        }
    }
}
