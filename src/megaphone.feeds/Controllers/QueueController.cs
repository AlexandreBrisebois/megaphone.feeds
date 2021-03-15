using Dapr.Client;
using Feeds.API.Commands;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Events;
using Megaphone.Standard.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Controllers
{
    [ApiController]
    [Route("/")]
    public class QueueController : ControllerBase
    {
        private readonly DaprClient daprClient;
        private readonly TelemetryClient telemetryClient;
        private readonly FeedStorageService feedStorageService;

        public QueueController([FromServices] DaprClient daprClient, TelemetryClient telemetryClient) : base()
        {
            this.daprClient = daprClient;
            this.telemetryClient = telemetryClient;

            feedStorageService = new FeedStorageService(daprClient);
        }

        [HttpPost("feed-requests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync(Event e)
        {
            if (e.Name == Events.Events.Feed.Add)
            {
                await AddFeed(e);
                telemetryClient.TrackEvent(Events.Events.Feed.Delete, new Dictionary<string, string> { { Events.Events.Feed.Add, e.Metadata.GetValueOrDefault("url") } });
            }
            else if (e.Name == Events.Events.Feed.Delete)
            {
                await DeleteFeed(e);
                telemetryClient.TrackEvent(Events.Events.Feed.Delete, new Dictionary<string, string> { { Events.Events.Feed.Delete, e.Metadata.GetValueOrDefault("id") } });
            }

            return Ok();
        }

        private async Task DeleteFeed(Event e)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            if (!entry.HasValue)
            {
                entry.Value = new List<Models.Feed>();
            }

            entry.Value = entry.Value.Where(i => i.Id != e.Metadata.GetValueOrDefault("id")).ToList();

            var c = new PersistFeedListCommand(entry);
            await c.ApplyAsync(feedStorageService);
        }

        private async Task AddFeed(Event e)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            if (!entry.HasValue)
            {
                entry.Value = new List<Models.Feed>();
            }

            var feed = new Models.Feed
            {
                Url = e.Metadata.GetValueOrDefault("url")
            };

            feed.Id = new Uri(feed.Url).ToGuid().ToString();
            entry.Value.Add(feed);

            var c = new PersistFeedListCommand(entry);
            await c.ApplyAsync(feedStorageService);

            telemetryClient.TrackEvent(Events.Events.Feed.UpdateFeedList);

            var publish = new SendCrawlRequestCommand(feed.Url);
            await publish.ApplyAsync(daprClient);

            telemetryClient.TrackEvent(Events.Events.Feed.SentCrawlRequest, new Dictionary<string, string> { { "url", feed.Url } });
        }
    }
}
