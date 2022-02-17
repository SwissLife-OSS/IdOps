using System.Diagnostics;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using IdOps.IdentityServer.Events;

namespace IdOps.IdentityServer.Samples
{
    public class ActivityEnricherSink : IIdOpsEventSink
    {
        public ValueTask ProcessAsync(Event evt, Activity? activity)
        {
            if (evt.EventType is EventTypes.Failure or EventTypes.Error && activity is { })
            {
                var tagsCollection = new ActivityTagsCollection
                {
                    { "exception.category", evt.Category },
                    { "exception.name", evt.Name },
                    { "exception.type", evt.GetType().FullName },
                };

                if (!string.IsNullOrWhiteSpace(evt.Message))
                {
                    tagsCollection.Add("exception.message", evt.Message);
                }

                activity.AddEvent(new ActivityEvent("exception", default, tagsCollection));
            }

            return ValueTask.CompletedTask;
        }
    }
}
