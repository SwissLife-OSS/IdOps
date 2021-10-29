using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Store.Mongo
{
    internal class ApiResourceCollectionConfiguration :
        IMongoCollectionConfiguration<IdOpsApiResource>
    {
        private readonly string _collectionName;

        public ApiResourceCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<IdOpsApiResource> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<IdOpsApiResource>(cm =>
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
