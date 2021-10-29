using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;

namespace IdOps.IdentityServer.Events
{
    public static class EventExtensions
    {
        public static string? TryGetPropertyValue(this Event evt, string name)
        {
           PropertyInfo? property = evt.GetType().GetProperty(name);

            return property?.GetValue(evt)?.ToString();
        }

        public static Task RaiseAsync(
            this Event @event,
            IEventService service)
        {
            return service.RaiseAsync(@event);
        }
    }
}
