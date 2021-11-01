using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdOps.IdentityServer.Storage.Mongo
{
    public class ResourceUpdater<T>
        where T : IdOpsResource
    {
        public ResourceUpdater(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public IMongoCollection<T> _collection { get; }

        public async Task<UpdateResourceResult> UpdateAsync(
            T resource,
            CancellationToken cancellationToken)
        {
            T? current = await _collection.AsQueryable()
                .Where(x => x.Source.Id == resource.Source.Id)
                .SingleOrDefaultAsync(cancellationToken);

            var result = new UpdateResourceResult()
            {
                NewVersion = resource.Source.Version
            };

            if (current is { })
            {
                result.OldVersion = current.Source.Version;

                if (resource.Source.Version > current.Source.Version)
                {
                    await _collection.ReplaceOneAsync(
                        x => x.Source.Id == resource.Source.Id,
                        resource,
                        new ReplaceOptions(),
                        cancellationToken);

                    result.Operation = UpdateResourceOperation.Updated;
                }
            }
            else
            {
                await _collection.InsertOneAsync(
                    resource,
                    options: null,
                    cancellationToken);

                result.Operation = UpdateResourceOperation.Created;
            }

            return result;
        }
    }
}
