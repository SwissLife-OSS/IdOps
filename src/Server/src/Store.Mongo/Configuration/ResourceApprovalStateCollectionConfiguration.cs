using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class ResourceApprovalStateCollectionConfiguration :
        IMongoCollectionConfiguration<ResourceApprovalState>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ResourceApprovalState> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ResourceApprovalState)
                .AddBsonClassMap<ResourceApprovalState>(cm =>
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
