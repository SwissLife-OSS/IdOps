using MongoDB.Driver;

namespace IdOps.IdentityServer.Storage.Mongo;

public static class MongoCollations
{
    public static Collation CaseInsensitive { get; } =
        new Collation(locale: "en", strength: CollationStrength.Secondary);
}
