using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class ResourceConsumer
        : IResourceConsumer
    {
        private readonly Dictionary<string, IResourceMessageConsumer> _lookup;

        public ResourceConsumer(IEnumerable<IResourceMessageConsumer> consumers)
        {
            _lookup = consumers.ToDictionary(x => x.MessageType);
        }

        public async Task<UpdateResourceResult> ProcessAsync(
            string messageType,
            byte[] data,
            CancellationToken cancellationToken)
        {
            if (!_lookup.TryGetValue(messageType, out IResourceMessageConsumer? consumer))
            {
                throw new InvalidOperationException("UnknownType");
            }

            return await consumer.ProcessAsync(data, cancellationToken);
        }
    }
}
