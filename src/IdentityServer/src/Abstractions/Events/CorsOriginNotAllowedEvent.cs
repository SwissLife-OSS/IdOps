using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public class CorsOriginNotAllowedEvent : Event
    {
        public CorsOriginNotAllowedEvent(
            string? origin,
            IEnumerable<string> allowed)
            : base(
                "Cors",
                $"{origin} not allowed",
                EventTypes.Failure,
                EventIds.CorsOriginNotAllowed)
        {
            Origin = origin;
            AllowedOrigins = allowed;
        }

        public string? Origin { get; }

        public IEnumerable<string> AllowedOrigins { get; set; }
    }
}
