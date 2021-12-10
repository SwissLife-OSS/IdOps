using System;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
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
                    var searchIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys
                             .Ascending(c => c.EnvironmentName)
                             .Ascending(c => c.ClientId)
                             .Ascending(c => c.EventType)
                             .Descending(c => c.TimeStamp),
                         new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(searchIndex);

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
