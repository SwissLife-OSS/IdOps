using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Store.Mongo
{
    internal class IdentityResourceCollectionConfiguration :
        IMongoCollectionConfiguration<IdOpsIdentityResource>
    {
        private readonly string _collectionName;

        public IdentityResourceCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<IdOpsIdentityResource> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<IdOpsIdentityResource>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.AutoMap();
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
