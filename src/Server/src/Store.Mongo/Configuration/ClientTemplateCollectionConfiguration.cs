using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ClientTemplateCollectionConfiguration :
        IMongoCollectionConfiguration<ClientTemplate>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ClientTemplate> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ClientTemplate)
                .AddBsonClassMap<ClientTemplate>(cm =>
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
