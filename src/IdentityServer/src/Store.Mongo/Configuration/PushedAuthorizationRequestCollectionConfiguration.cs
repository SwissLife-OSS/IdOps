using System;
using Duende.IdentityServer.Models;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Storage.Mongo
{
    internal class PushedAuthorizationRequestCollectionConfiguration
        : IMongoCollectionConfiguration<PushedAuthorizationRequest>
    {
        private readonly string _collectionName;

        public PushedAuthorizationRequestCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<PushedAuthorizationRequest> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<PushedAuthorizationRequest>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.ReferenceValueHash);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var ttlIndex = new CreateIndexModel<PushedAuthorizationRequest>(
                        Builders<PushedAuthorizationRequest>.IndexKeys
                            .Ascending(c => c.ExpiresAtUtc),
                        new CreateIndexOptions
                        {
                            Name = "ttl_expiration_v1",
                            Unique = false,
                            ExpireAfter = TimeSpan.Zero
                        });

                    collection.Indexes.CreateOne(ttlIndex);
                });
        }
    }
}
