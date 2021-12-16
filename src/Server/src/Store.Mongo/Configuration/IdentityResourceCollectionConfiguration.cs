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
                    var uniquenessIndex = new CreateIndexModel<IdentityResource>(
                        Builders<IdentityResource>.IndexKeys
                            .Ascending(c => c.Name)
                            .Ascending(c => c.IdentityServerGroupId),
                        new CreateIndexOptions { Unique = true });

                    var nameIndex = new CreateIndexModel<IdentityResource>(
                         Builders<IdentityResource>.IndexKeys
                             .Ascending(c => c.Name),
                         new CreateIndexOptions { Unique = false });

                    var tenantsIndex = new CreateIndexModel<IdentityResource>(
                        Builders<IdentityResource>.IndexKeys
                            .Ascending(c => c.Tenants),
                        new CreateIndexOptions { Unique = false });

                    var serverGroupIndex = new CreateIndexModel<IdentityResource>(
                        Builders<IdentityResource>.IndexKeys
                            .Ascending(c => c.IdentityServerGroupId),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { uniquenessIndex, nameIndex, tenantsIndex, serverGroupIndex });
                });
        }
    }
}
