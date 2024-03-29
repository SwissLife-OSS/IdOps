﻿using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace IdOps.Server.Storage.Mongo.Configuration
{
    internal class UserClaimRuleCollectionConfiguration :
        IMongoCollectionConfiguration<UserClaimRule>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<UserClaimRule> builder)
        {
            builder
                .WithCollectionName(CollectionNames.UserClaimRule)
                .AddBsonClassMap<UserClaimRule>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var nameIndex = new CreateIndexModel<UserClaimRule>(
                     Builders<UserClaimRule>.IndexKeys
                         .Ascending(c => c.Name)
                         .Ascending(c => c.Tenant),
                     new CreateIndexOptions { Unique = true });

                    collection.Indexes.CreateOne(nameIndex);
                });
        }
    }
}
