using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdOps.IdentityServer.Extensions;
using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer.Events
{
    public class IdOpsEventSink : IEventSink
    {
        private readonly ILogger<BusEventSink> _logger;
        private readonly IEnumerable<IIdOpsEventSink> _sinks;

        public IdOpsEventSink(
            ILogger<IdOpsEventSink> logger, 
            IEnumerable<IIdOpsEventSink> sinks)
        {
            _logger = logger;
            _sinks = sinks;
        }

        public Task PersistAsync(Event evt)
        {
            try
            {
                Task.Run(() => ProcessAsync(evt)).Forget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending event");
            }

            return Task.CompletedTask;
        }

        private async Task ProcessAsync(Event evt)
        {
            foreach (IIdOpsEventSink sink in _sinks)
            {
                try
                {
                    await sink.ProcessAsync(evt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while processing event {sink.GetType().FullName}");
                }
            }
        }
    }
}
