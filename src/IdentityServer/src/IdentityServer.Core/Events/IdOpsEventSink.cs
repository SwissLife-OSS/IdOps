using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer.Events
{
    public class IdOpsEventSink : IEventSink
    {
        private readonly ILogger<IdOpsEventSink> _logger;
        private readonly IEnumerable<IIdOpsEventSink> _sinks;

        public IdOpsEventSink(
            ILogger<IdOpsEventSink> logger, 
            IEnumerable<IIdOpsEventSink> sinks)
        {
            _logger = logger;
            _sinks = sinks;
        }

        public async Task PersistAsync(Event evt)
        {
            try
            {
                Activity? activity = Activity.Current;

                foreach (IIdOpsEventSink sink in _sinks)
                {
                    await sink.ProcessAsync(evt, activity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending event");
            }
        }
    }
}
