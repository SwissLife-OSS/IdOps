using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ApiResourceCollectionConfiguration : IMongoCollectionConfiguration<ApiResource>
    {
        public void OnConfiguring(IMongoCollectionBuilder<ApiResource> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ApiResource)
                .AddBsonClassMap<ApiResource>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var nameIndex = new CreateIndexModel<ApiResource>(
                        Builders<ApiResource>.IndexKeys
                            .Ascending(c => c.Name)
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });

                    var scopesIndex = new CreateIndexModel<ApiResource>(
                        Builders<ApiResource>.IndexKeys.Ascending(c => c.Scopes),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { nameIndex, scopesIndex });
                });
        }
    }
}
