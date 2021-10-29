using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Store.Mongo
{
    internal class UserClaimRuleCollectionConfiguration :
        IMongoCollectionConfiguration<UserClaimRule>
    {
        private readonly string _collectionName;

        public UserClaimRuleCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<UserClaimRule> builder)
        {
            builder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<UserClaimRule>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
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
