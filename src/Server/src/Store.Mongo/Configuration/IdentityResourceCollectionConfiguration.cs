using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class IdentityResourceCollectionConfiguration :
        IMongoCollectionConfiguration<IdentityResource>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<IdentityResource> builder)
        {
            builder
                .WithCollectionName(CollectionNames.IdentityResource)
                .AddBsonClassMap<IdentityResource>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var nameIndex = new CreateIndexModel<IdentityResource>(
                         Builders<IdentityResource>.IndexKeys
                             .Ascending(c => c.Name)
                             .Ascending(c => c.IdentityServerGroupId),
                         new CreateIndexOptions { Unique = true });

                    collection.Indexes.CreateOne(nameIndex);
                });
        }
    }
}
