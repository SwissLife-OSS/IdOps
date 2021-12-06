using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ResourcePublishLogCollectionConfiguration :
        IMongoCollectionConfiguration<ResourcePublishLog>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ResourcePublishLog> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ResourcePublishLog)
                .AddBsonClassMap<ResourcePublishLog>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var resourceIdIndex = new CreateIndexModel<ResourcePublishLog>(
                        Builders<ResourcePublishLog>.IndexKeys
                            .Ascending(c => c.ResourceId),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(resourceIdIndex);
                });
        }
    }
}
