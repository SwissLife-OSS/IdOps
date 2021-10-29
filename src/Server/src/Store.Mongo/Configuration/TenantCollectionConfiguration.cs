using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class TenantCollectionConfiguration :
        IMongoCollectionConfiguration<Tenant>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Tenant> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Tenant)
                .AddBsonClassMap<Tenant>(cm =>
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
