using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class ResourceAuditEventCollectionConfiguration :
        IMongoCollectionConfiguration<ResourceAuditEvent>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ResourceAuditEvent> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ResourceAudit)
                .AddBsonClassMap<ResourceAuditEvent>(cm =>
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
