using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class ResourcePublishStateCollectionConfiguration :
        IMongoCollectionConfiguration<ResourcePublishState>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ResourcePublishState> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ResourcePublishState)
                .AddBsonClassMap<ResourcePublishState>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
