using MongoDB.Extensions.Context;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class IdOpsMongoOptions : MongoOptions
    {
        public IdOpsMongoOptions()
        {
        }

        public CollectionNames CollectionNames { get; } = new CollectionNames();
    }
}
