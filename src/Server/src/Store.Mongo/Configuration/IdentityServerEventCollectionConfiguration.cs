using System;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Store.Mongo.Configuration
{
    internal class IdentityServerEventCollectionConfiguration :
        IMongoCollectionConfiguration<IdentityServerEvent>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<IdentityServerEvent> builder)
        {
            builder
                .WithCollectionName(CollectionNames.IdentityServerEvent)
                .AddBsonClassMap<IdentityServerEvent>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var envIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys
                             .Ascending(c => c.EnvironmentName)
                             .Descending(c => c.TimeStamp),
                         new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(envIndex);

                    var clientIdIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys
                             .Ascending(c => c.ClientId)
                             .Descending(c => c.TimeStamp),
                         new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(clientIdIndex);

                    var ttlIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys.Ascending(c => c.TimeStamp),
                         new CreateIndexOptions
                         {
                             Unique = false,
                             ExpireAfter = TimeSpan.FromDays(90)
                         });

                    collection.Indexes.CreateOne(ttlIndex);
                });
        }
    }
}
