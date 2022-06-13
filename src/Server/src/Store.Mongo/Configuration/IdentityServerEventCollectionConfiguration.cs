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
                .WithCollectionSettings(s => s.WriteConcern = WriteConcern.W1.With(journal: false))
                .WithCollectionConfiguration(collection =>
                {
                    collection.Indexes.DropOne("_id_");

                    var searchAllIndex = new CreateIndexModel<IdentityServerEvent>(
                        Builders<IdentityServerEvent>.IndexKeys
                            .Ascending(c => c.EnvironmentName)
                            .Ascending(c => c.EventType)
                            .Descending(c => c.TimeStamp),
                        new CreateIndexOptions
                        {
                            Name = "search_all",
                            Unique = false
                        });

                    collection.Indexes.CreateOne(searchAllIndex);

                    var searchIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys
                             .Ascending(c => c.ClientId)
                             .Ascending(c => c.EnvironmentName)
                             .Ascending(c => c.EventType)
                             .Descending(c => c.TimeStamp),
                         new CreateIndexOptions
                         {
                             Name = "search_by_clients",
                             Unique = false
                         });

                    collection.Indexes.CreateOne(searchIndex);

                    var ttlIndex = new CreateIndexModel<IdentityServerEvent>(
                         Builders<IdentityServerEvent>.IndexKeys.Ascending(c => c.TimeStamp),
                         new CreateIndexOptions
                         {
                             Name = "timestamp_asc_ttl45d",
                             Unique = false,
                             ExpireAfter = TimeSpan.FromDays(45)
                         });

                    collection.Indexes.CreateOne(ttlIndex);
                });
        }
    }
}
