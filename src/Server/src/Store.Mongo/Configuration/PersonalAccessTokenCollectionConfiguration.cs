using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using static MongoDB.Driver.Builders<IdOps.Model.PersonalAccessToken>;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class PersonalAccessTokenCollectionConfiguration
        : IMongoCollectionConfiguration<PersonalAccessToken>
    {
        public void OnConfiguring(IMongoCollectionBuilder<PersonalAccessToken> builder)
        {
            builder
                .WithCollectionName(CollectionNames.PersonalAccessToken)
                .AddBsonClassMap<PersonalAccessToken>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    CreateIndexModel<PersonalAccessToken> tokenIndex = new(
                        IndexKeys.Ascending($"{nameof(PersonalAccessToken.Tokens)}.{nameof(HashedToken.Token)}"));

                    var tenantIndex = new CreateIndexModel<PersonalAccessToken>(
                        Builders<PersonalAccessToken>.IndexKeys
                            .Ascending(c => c.Tenant),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { tokenIndex, tenantIndex });
                });
        }
    }
}
