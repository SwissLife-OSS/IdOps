using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class IdentityServerCollectionConfiguration :
        IMongoCollectionConfiguration<Model.IdentityServer>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Model.IdentityServer> builder)
        {
            builder
                .WithCollectionName(CollectionNames.IdentityServer)
                .AddBsonClassMap<Model.IdentityServer>(cm =>
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
