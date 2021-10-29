using System;
using Duende.IdentityServer.Models;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace  IdOps.IdentityServer.Store.Mongo
{
    internal class PersistedGrantCollectionConfiguration
        : IMongoCollectionConfiguration<PersistedGrant>
    {
        public void OnConfiguring(IMongoCollectionBuilder<PersistedGrant> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName("grant")
                .AddBsonClassMap<PersistedGrant>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Key);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var ttlIndex = new CreateIndexModel<PersistedGrant>(
                        Builders<PersistedGrant>.IndexKeys.Ascending(c => c.CreationTime),
                        new CreateIndexOptions
                        {
                            Unique = false,
                            ExpireAfter = TimeSpan.FromDays(365)
                        });
                    try
                    {
                        collection.Indexes.CreateOne(ttlIndex);
                    }
                    catch (Exception ex)
                    {
                    }
                });
        }
    }
}
