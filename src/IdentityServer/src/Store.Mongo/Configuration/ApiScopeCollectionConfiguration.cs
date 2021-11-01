using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace  IdOps.IdentityServer.Storage.Mongo
{
    internal class ApiScopeCollectionConfiguration
        : IMongoCollectionConfiguration<IdOpsApiScope>
    {
        private readonly string _collectionName;

        public ApiScopeCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(IMongoCollectionBuilder<IdOpsApiScope> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<IdOpsApiScope>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.AutoMap();
                })
                .AddBsonClassMap<Resource>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Name);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
