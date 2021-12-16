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
                    var uniquenessIndex = new CreateIndexModel<ApiResource>(
                        Builders<ApiResource>.IndexKeys
                            .Ascending(c => c.Name)
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });

                    var nameIndex = new CreateIndexModel<ApiResource>(
                        Builders<ApiResource>.IndexKeys
                            .Ascending(c => c.Name),
                        new CreateIndexOptions { Unique = false });

                    var tenantIndex = new CreateIndexModel<ApiResource>(
                        Builders<ApiResource>.IndexKeys
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { uniquenessIndex, nameIndex, tenantIndex });
                });
        }
    }
}
