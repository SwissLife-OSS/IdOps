using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
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

                    collection.Indexes.CreateOne(nameIndex);
                });
        }
    }
}
