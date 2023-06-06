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
                    string tokenIndexSelector =
                        $"{nameof(PersonalAccessToken.Tokens)}.{nameof(HashedToken.Token)}";

                    CreateIndexModel<PersonalAccessToken> tokenIndex = new(
                        IndexKeys.Ascending(tokenIndexSelector));

                    CreateIndexModel<PersonalAccessToken> scopesIndex = new(
                        IndexKeys.Ascending(c => c.AllowedScopes),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateMany(new[] { tokenIndex, scopesIndex });
                });
        }
    }
}
