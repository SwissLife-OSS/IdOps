using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

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
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest);
        }
    }
}
