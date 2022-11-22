using System;
using Duende.IdentityServer.Models;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace  IdOps.IdentityServer.Storage.Mongo
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

                    var persistedGrantFilter = new CreateIndexModel<PersistedGrant>(
                        Builders<PersistedGrant>.IndexKeys
                            .Ascending(g => g.ClientId)
                            .Ascending(g => g.SubjectId)
                            .Ascending(g => g.Type));
                    try
                    {
                        collection.Indexes.CreateMany(new [] { ttlIndex, persistedGrantFilter});
                    }
                    catch (Exception ex)
                    {
                    }
                });
        }
    }
}
