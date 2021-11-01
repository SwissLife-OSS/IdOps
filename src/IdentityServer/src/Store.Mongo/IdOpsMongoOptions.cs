using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Storage.Mongo
{
    public class IdOpsMongoOptions : MongoOptions
    {
        public IdOpsMongoOptions()
        {
        }

        public CollectionNames CollectionNames { get; } = new CollectionNames();
    }
}
