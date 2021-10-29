using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdOps.Model
{
    public class IdentityServerEvent 
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public int EventId { get; set; }
        public string? Message { get; set; }
        public string EnvironmentName { get; set; }
        public string ServerGroupName { get; set; }
        public string? ActivityId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ProcessId { get; set; }
        public string? LocalIpAddress { get; set; }
        public string? RemoteIpAddress { get; set; }
        public string Hostname { get; set; }
        public string? ClientId { get; set; }
        public string? SubjectId { get; set; }
        public string? Category { get; set; }
        public string? EventType { get; set; }
        public string? Endpoint { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }
}
