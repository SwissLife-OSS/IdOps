using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel.Client;
using IdOps.IdentityServer.Extensions;
using IdOps.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer.Events
{
    public class BusEventSink : IEventSink
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdOpsOptions _idOpsOptions;
        private readonly ILogger<BusEventSink> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };

        public BusEventSink(
            IBus bus,
            IHttpContextAccessor httpContextAccessor,
            IdOpsOptions idOpsOptions,
            ILogger<BusEventSink> logger)
        {
            _bus = bus;
            _httpContextAccessor = httpContextAccessor;
            _idOpsOptions = idOpsOptions;
            _logger = logger;
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public Task PersistAsync(Event evt)
        {
            try
            {
                Task.Run(() => SendEventAsync(evt)).Forget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending event");
            }

            return Task.CompletedTask;
        }

        private async Task SendEventAsync(Event evt)
        {
            evt.RemoteIpAddress = _httpContextAccessor.HttpContext?.GetRemoteIpAddress();

            var entity = new IdentityEventMessage
            {
                EnvironmentName = _idOpsOptions.EnvironmentName,
                ServerGroup = _idOpsOptions.ServerGroup,
                Type = evt.GetType().FullName,
                Hostname = Environment.MachineName,
                Data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<object>(evt, _jsonOptions)),
            };

            await _bus.Publish(entity);
        }
    }
}
