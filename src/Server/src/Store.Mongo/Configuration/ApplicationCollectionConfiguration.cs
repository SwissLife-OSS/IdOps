using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ApplicationCollectionConfiguration :
        IMongoCollectionConfiguration<Application>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Application> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Application)
                .AddBsonClassMap<Application>(cm =>
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
