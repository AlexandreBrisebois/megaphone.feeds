using Feeds.API.Commands;
using Megaphone.Feeds.Messages;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Extensions;
using Megaphone.Standard.Messages;
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
        private readonly TelemetryClient telemetryClient;
        private readonly ICrawlerService crawlerService;
        private readonly IFeedService feedService;

        public QueueController([FromServices] TelemetryClient telemetryClient,
                               [FromServices] IFeedService feedService,
                               [FromServices] ICrawlerService crawlerService) : base()
        {
            this.crawlerService = crawlerService;
            this.telemetryClient = telemetryClient;

            this.feedService = feedService;
        }

        [HttpPost("feed-requests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync(CommandMessage message)
        {
            if (message.Action == Actions.Feed.Add)
            {
                await AddFeed(message);

                telemetryClient.TrackEvent(Actions.Feed.Delete, new Dictionary<string, string>
                                                                {
                                                                    { "url", message.Parameters.GetValueOrDefault("url") },
                                                                    { "display", message.Parameters.GetValueOrDefault("display") }
                                                                });

            }
            else if (message.Action == Actions.Feed.Delete)
            {
                await DeleteFeed(message);
                telemetryClient.TrackEvent(Actions.Feed.Delete, new Dictionary<string, string> { { Actions.Feed.Delete, message.Parameters.GetValueOrDefault("id") } });
            }

            return Ok();
        }

        private async Task DeleteFeed(CommandMessage message)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedService);

            if (!entry.HasValue)
            {
                entry.Value = new List<Models.Feed>();
            }

            entry.Value = entry.Value.Where(i => i.Id != message.Parameters.GetValueOrDefault("id")).ToList();

            var c = new PersistFeedListCommand(entry);
            await c.ApplyAsync(feedService);
        }

        private async Task AddFeed(CommandMessage message)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedService);

            if (!entry.HasValue)
            {
                entry.Value = new List<Models.Feed>();
            }

            string url = message.Parameters.GetValueOrDefault("url");
            string id = new Uri(url).ToGuid().ToString();
            string display = message.Parameters.GetValueOrDefault("display");

            var feed = entry.Value.Find(f => f.Id == id);

            if (feed != null)
            {
                feed.Display = display;
            }
            else
            {
                feed = new Models.Feed
                {
                    Url = url,
                    Display = display,
                    Id = id
                };

                entry.Value.Add(feed);
            }

            var c = new PersistFeedListCommand(entry);
            await c.ApplyAsync(feedService);

            telemetryClient.TrackEvent(Events.Events.Feed.UpdateFeedList);

            var publish = new SendCrawlRequestCommand(feed);
            await publish.ApplyAsync(crawlerService);

            telemetryClient.TrackEvent(Events.Events.Feed.SentCrawlRequest, new Dictionary<string, string> { { "url", feed.Url } });
        }
    }
}
