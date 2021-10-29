using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class EnvironmentCollectionConfiguration :
        IMongoCollectionConfiguration<Environment>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Environment> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Environment)
                .AddBsonClassMap<Environment>(cm =>
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
