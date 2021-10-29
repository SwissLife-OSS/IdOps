using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class SecureSecretCollectionConfiguration :
        IMongoCollectionConfiguration<SecureSecret>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<SecureSecret> builder)
        {
            builder
                .WithCollectionName(CollectionNames.SecureSecret)
                .AddBsonClassMap<SecureSecret>(cm =>
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
