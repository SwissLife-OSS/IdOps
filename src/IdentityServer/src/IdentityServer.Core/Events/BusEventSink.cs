using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using IdOps.IdentityServer.Extensions;
using IdOps.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace IdOps.IdentityServer.Events
{
    public class BusEventSink : IIdOpsEventSink
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdOpsOptions _idOpsOptions;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };

        public BusEventSink(
            IBus bus,
            IHttpContextAccessor httpContextAccessor,
            IdOpsOptions idOpsOptions)
        {
            _bus = bus;
            _httpContextAccessor = httpContextAccessor;
            _idOpsOptions = idOpsOptions;
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task ProcessAsync(Event evt)
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
