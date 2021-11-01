using System;
using IdOps.IdentityServer.DataConnector;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Storage.Mongo
{
    internal class UserDataConnectorDataCollectionConfiguration :
        IMongoCollectionConfiguration<UserDataConnectorData>
    {
        private readonly string _collectionName;

        public UserDataConnectorDataCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }


        public void OnConfiguring(
            IMongoCollectionBuilder<UserDataConnectorData> builder)
        {
            builder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<UserDataConnectorData>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Key);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var ttlIndex = new CreateIndexModel<UserDataConnectorData>(
                        Builders<UserDataConnectorData>.IndexKeys.Ascending(c => c.LastModifiedAt),
                        new CreateIndexOptions
                        {
                            Unique = false,
                            ExpireAfter = TimeSpan.FromDays(7)
                        });

                    collection.Indexes.CreateOne(ttlIndex);
                });
        }
    }
}
