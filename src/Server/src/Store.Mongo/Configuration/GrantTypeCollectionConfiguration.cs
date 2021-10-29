using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class GrantTypeCollectionConfiguration :
        IMongoCollectionConfiguration<GrantType>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<GrantType> builder)
        {
            builder
                .WithCollectionName(CollectionNames.GrantType)
                .AddBsonClassMap<GrantType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
