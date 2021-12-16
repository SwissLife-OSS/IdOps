using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ApiScopeCollectionConfiguration :
        IMongoCollectionConfiguration<ApiScope>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ApiScope> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ApiScope)
                .AddBsonClassMap<ApiScope>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var uniquenessIndex = new CreateIndexModel<ApiScope>(
                        Builders<ApiScope>.IndexKeys
                            .Ascending(c => c.Name)
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = true });

                    var nameIndex = new CreateIndexModel<ApiScope>(
                         Builders<ApiScope>.IndexKeys
                             .Ascending(c => c.Name),
                         new CreateIndexOptions { Unique = false });

                    var tenantIndex = new CreateIndexModel<ApiScope>(
                        Builders<ApiScope>.IndexKeys
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { uniquenessIndex, nameIndex, tenantIndex });
                });
        }
    }
}
