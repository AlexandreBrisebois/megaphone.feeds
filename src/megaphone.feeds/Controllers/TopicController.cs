using Dapr;
using Dapr.Client;
using Megaphone.Feeds.Commands;
using Megaphone.Feeds.Events;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Events;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Controllers
{
    [ApiController]
    [Route("/")]
    public class TopicController : ControllerBase
    {
        private readonly ResourceStorageService resourceStorageService;
        private readonly ResourceListChangeTracker resourceTracker;
        private readonly TelemetryClient telemetryClient;
        private readonly FeedStorageService feedStorageService;

        public TopicController([FromServices] DaprClient daprClient,
                               [FromServices] ResourceListChangeTracker resourceTracker,
                               TelemetryClient telemetryClient)
        {
            resourceStorageService = new ResourceStorageService(daprClient);
            feedStorageService = new FeedStorageService(daprClient);

            this.resourceTracker = resourceTracker;
            this.telemetryClient = telemetryClient;
        }

        [HttpPost("resource-events")]
        [Topic("resource-events", "resource-events")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync(Event e)
        {
            if (e.Name == Events.Events.Resource.Update)
            {
                if (e.TryConvertToFeed(out var f))
                    await UpdateFeed(f);
                else if (e.TryConvertToResource(out var r))
                    await UpsertResource(r);
            }
            return Ok();
        }

        private async Task UpsertResource(Resource r)
        {
            var c = new UpsertResourceListCommand(r);
            await c.ApplyAsync(resourceStorageService);

            telemetryClient.TrackEvent(Events.Events.Resource.Update, new Dictionary<string, string> { { "display", r.Display }, { "url", r.Url } });

            resourceTracker.AddDate(r.Published);
        }

        private async Task UpdateFeed(Feed f)
        {
            var c = new UpdateFeedListCommand(f);
            await c.ApplyAsync(feedStorageService);

            telemetryClient.TrackEvent(Events.Events.Feed.UpdateFeedList, new Dictionary<string, string> { { "display", f.Display }, { "url", f.Url } });

        }
    }
}
