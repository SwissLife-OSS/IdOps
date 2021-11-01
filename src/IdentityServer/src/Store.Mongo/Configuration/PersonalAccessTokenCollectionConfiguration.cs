using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using static MongoDB.Driver.Builders<IdOps.IdentityServer.Model.IdOpsPersonalAccessToken>;

namespace IdOps.IdentityServer.Storage.Mongo
{
    internal class PersonalAccessTokenCollectionConfiguration
        : IMongoCollectionConfiguration<IdOpsPersonalAccessToken>
    {
        private readonly string _collectionName;

        public PersonalAccessTokenCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<IdOpsPersonalAccessToken> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<IdOpsPersonalAccessToken>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdField(s => s.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(ConfigureCollection);
        }

        private static void ConfigureCollection(IMongoCollection<IdOpsPersonalAccessToken> collection)
        {
            CreateIndexModel<IdOpsPersonalAccessToken>[] index =
            {
                new(IndexKeys.Combine(IndexKeys.Ascending(WellKnownPatFields.IsUsed),
                        IndexKeys.Ascending(WellKnownPatFields.ExpiresAt),
                        IndexKeys.Ascending(x => x.UserName)),
                    new CreateIndexOptions { Unique = true, Name = "PAT-Token-Search" })
            };

            collection.Indexes.CreateMany(index);
        }
    }
}
