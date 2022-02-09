using System.Diagnostics;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using IdOps.IdentityServer.Events;

namespace IdOps.IdentityServer.Samples
{
    public class ActivityEnricherSink : IIdOpsEventSink
    {
        public Task ProcessAsync(Event evt)
        {
            if (evt.EventType is EventTypes.Failure or EventTypes.Error)
            {
                Activity? activity = Activity.Current;
                if (activity is null)
                {
                    return Task.CompletedTask;
                }

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

            return Task.CompletedTask;
        }
    }
}
