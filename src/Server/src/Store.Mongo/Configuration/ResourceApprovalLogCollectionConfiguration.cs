using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ResourceApprovalLogCollectionConfiguration :
        IMongoCollectionConfiguration<ResourceApprovalLog>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ResourceApprovalLog> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ResourceApprovalLog)
                .AddBsonClassMap<ResourceApprovalLog>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var resourceIdIndex = new CreateIndexModel<ResourceApprovalLog>(
                        Builders<ResourceApprovalLog>.IndexKeys
                            .Ascending(c => c.ResourceId),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(resourceIdIndex);
                });
        }
    }
}
