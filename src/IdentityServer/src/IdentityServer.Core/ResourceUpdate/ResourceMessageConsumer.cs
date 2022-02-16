using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public abstract class ResourceMessageConsumer<T> : IResourceMessageConsumer
    {
        public abstract string MessageType { get; }

        public async Task<UpdateResourceResult> ProcessAsync(
            byte[] data,
            CancellationToken cancellationToken)
        {
            T deserialized = await DeserializeAsync(data, cancellationToken);
            return await Process(deserialized, cancellationToken);
        }

        public abstract Task<UpdateResourceResult> Process(
            T data,
            CancellationToken cancellationToken);

        private static async Task<T> DeserializeAsync(
            byte[] data,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(data);
            JsonSerializerOptions options = new();
            T? res = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);

            if (res == null)
            {
                throw new ApplicationException($"Could not deserialize to: {typeof(T)}");
            }

            return res;
        }
    }
}
