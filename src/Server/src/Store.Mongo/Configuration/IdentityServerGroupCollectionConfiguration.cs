using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class IdentityServerGroupCollectionConfiguration :
        IMongoCollectionConfiguration<IdentityServerGroup>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<IdentityServerGroup> builder)
        {
            builder
                .WithCollectionName(CollectionNames.IdentityServerGroup)
                .AddBsonClassMap<IdentityServerGroup>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var nameIndex = new CreateIndexModel<IdentityServerGroup>(
                         Builders<IdentityServerGroup>.IndexKeys
                             .Ascending(c => c.Name),
                         new CreateIndexOptions { Unique = true });

                    collection.Indexes.CreateOne(nameIndex);
                });
        }
    }
}
