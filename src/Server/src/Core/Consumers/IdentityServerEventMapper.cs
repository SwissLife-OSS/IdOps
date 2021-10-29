using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using IdOps.Messages;
using IdOps.Model;

namespace IdOps.Consumers
{
    public class IdentityServerEventMapper : IIdentityServerEventMapper
    {
        private HashSet<string> _mutedClients;

        public IdentityServerEventMapper(IEnumerable<string> mutedClients)
        {
            _mutedClients = new HashSet<string>(mutedClients);
        }

        public IdentityServerEvent? CreateEvent(IdentityEventMessage message)
        {
            var json = Encoding.UTF8.GetString(message.Data);

            IdentityServerEvent? isEvent = JsonSerializer
                .Deserialize<IdentityServerEvent>(json);

            if (isEvent == null)
            {
                return null;
            }

            isEvent.EnvironmentName = message.EnvironmentName;
            isEvent.Hostname = message.Hostname;
            isEvent.Type = message.Type;
            isEvent.ServerGroupName = message.ServerGroup;

            var properties = isEvent.GetType().GetProperties()
                .Select(x => x.Name)
                .ToList();
            var jsonDoc = JsonDocument.Parse(json);
            var data = new Dictionary<string, object>();

            foreach (JsonProperty property in jsonDoc.RootElement.EnumerateObject())
            {
                if ( property.Name == "Id")
                {
                    isEvent.EventId = property.Value.GetInt32();
                }

                if (!properties.Contains(property.Name))
                {
                    object? value = null;

                    switch (property.Value.ValueKind)
                    {
                        case JsonValueKind.String:
                            value = property.Value.GetString();
                            break;
                        case JsonValueKind.Number:
                            value = property.Value.GetDouble();
                            break;
                        case JsonValueKind.False:
                        case JsonValueKind.True:
                            value = property.Value.GetBoolean();
                            break;
                        case JsonValueKind.Array:
                            IEnumerable<string>? values = jsonDoc.RootElement
                                .TryGetStringArray(property.Name);
                            if (values is { } v && v.Any())
                            {
                                value = string.Join(",", values);
                            }

                            break;
                    }

                    if (value is { })
                    {
                        data.Add(property.Name, value);
                    }
                }
            }
            isEvent.Id = Guid.NewGuid();
            isEvent.Data = data;

            if (IsMuted(isEvent))
            {
                return null;
            }

            return isEvent;
        }

        private bool IsMuted(IdentityServerEvent isEvent)
        {
            return isEvent.ClientId != null && _mutedClients.Contains(isEvent.ClientId);
        }
    }
}
