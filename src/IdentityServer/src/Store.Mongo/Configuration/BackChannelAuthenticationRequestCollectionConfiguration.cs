using System;
using IdOps.IdentityServer.Storage.Mongo.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Storage.Mongo
{
    internal class BackChannelAuthenticationRequestCollectionConfiguration
        : IMongoCollectionConfiguration<SerializedBackChannelAuthenticationRequest>
    {
        private readonly string _collectionName;

        public BackChannelAuthenticationRequestCollectionConfiguration(string collectionName)
        {
            _collectionName = collectionName;
        }

        public void OnConfiguring(
            IMongoCollectionBuilder<SerializedBackChannelAuthenticationRequest> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName(_collectionName)
                .AddBsonClassMap<SerializedBackChannelAuthenticationRequest>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.InternalId);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var ttlIndex = new CreateIndexModel<SerializedBackChannelAuthenticationRequest>(
                        Builders<SerializedBackChannelAuthenticationRequest>.IndexKeys
                            .Ascending(c => c.ExpiresAt),
                        new CreateIndexOptions
                        {
                            Name = "ttl_expiration_v1",
                            Unique = false,
                            ExpireAfter = TimeSpan.Zero
                        });

                    var clientSubjectIndex = new CreateIndexModel<SerializedBackChannelAuthenticationRequest>(
                        Builders<SerializedBackChannelAuthenticationRequest>.IndexKeys
                            .Ascending(c => c.ClientId)
                            .Ascending("SubjectClaims.Type")
                            .Ascending("SubjectClaims.Value"),
                        new CreateIndexOptions
                        {
                            Name = "client_subject_v1",
                            Unique = false
                        });

                    collection.Indexes.CreateMany(new[] { ttlIndex, clientSubjectIndex });
                });
        }
    }
}
