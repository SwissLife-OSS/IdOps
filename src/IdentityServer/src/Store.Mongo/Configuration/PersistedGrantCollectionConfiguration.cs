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
                        Builders<PersistedGrant>.IndexKeys.Ascending(c => c.Expiration),
                        new CreateIndexOptions
                        {
                            Name = "ttl_expiration_v1",
                            Unique = false,
                            ExpireAfter = TimeSpan.FromDays(1)
                        });

                    var persistedGrantFilter = new CreateIndexModel<PersistedGrant>(
                        Builders<PersistedGrant>.IndexKeys
                            .Ascending(g => g.ClientId)
                            .Ascending(g => g.SubjectId)
                            .Ascending(g => g.Type),
                        new CreateIndexOptions
                        {
                            Name = "filter_client_subject_type_v1",
                            Unique = false
                        });

                    collection.Indexes.CreateMany(new [] { ttlIndex, persistedGrantFilter});
                });
        }
    }
}
