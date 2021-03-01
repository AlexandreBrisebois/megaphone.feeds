using Dapr.Client;
using Feeds.API.Commands;
using Megaphone.Feeds.Events;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Events;
using Megaphone.Standard.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace megaphone.feeds.Controllers
{
    [ApiController]
    [Route("/")]
    public class QueueController : ControllerBase
    {
        private readonly DaprClient daprClient;
        private readonly FeedStorageService feedStorageService;

        public QueueController([FromServices] DaprClient daprClient) : base()
        {
            this.daprClient = daprClient;
            feedStorageService = new FeedStorageService(daprClient);
        }

        [HttpPost("feed-requests")]
        public async Task<IActionResult> PostAsync(Event e)
        {
            if (e.Name == Events.Feed.Add)
            {
                await AddFeed(e);
            }
            else if (e.Name == Events.Feed.Delete)
            {
                await DeleteFeed(e);
            }

            return Ok();
        }

        private async Task DeleteFeed(Event e)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            if (!entry.HasValue)
            {
                entry.Value = new List<Megaphone.Feeds.Models.Feed>();
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
                entry.Value = new List<Megaphone.Feeds.Models.Feed>();
            }

            var feed = new Megaphone.Feeds.Models.Feed
            {
                Url = e.Metadata.GetValueOrDefault("url")
            };

            feed.Id = new Uri(feed.Url).ToGuid().ToString();
            entry.Value.Add(feed);

            var c = new PersistFeedListCommand(entry);
            await c.ApplyAsync(feedStorageService);

            var publish = new SendCrawlRequestCommand(feed.Url);
            await publish.ApplyAsync(daprClient);
        }
    }
}
