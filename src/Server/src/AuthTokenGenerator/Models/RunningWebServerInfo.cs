using System;

namespace IdOps.Models
{
    public record RunningWebServerInfo(Guid Id, string Url)
    {
        public DateTime StartedAt { get; } = DateTime.UtcNow;

        public string? Title { get; init; }
    }
}
