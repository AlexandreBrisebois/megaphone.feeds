using Dapr;
using Dapr.Client;
using Megaphone.Feeds.Commands;
using Megaphone.Feeds.Events;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Events;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace megaphone.feeds.Controllers
{
    [ApiController]
    [Route("/")]
    public class TopicController : ControllerBase
    {
        private readonly ResourceStorageService resourceStorageService;
        private readonly ResourceListChangeTracker resourceTracker;
        private readonly FeedStorageService feedStorageService;

        public TopicController([FromServices] DaprClient daprClient,
                               [FromServices] ResourceListChangeTracker resourceTracker)
        {
            resourceStorageService = new ResourceStorageService(daprClient);
            feedStorageService = new FeedStorageService(daprClient);

            this.resourceTracker = resourceTracker;
        }

        [Topic("resource-events", "resource-events")]
        [HttpPost("resource-updates")]
        public async Task<IActionResult> PostAsync(Event e)
        {
            if (e.Name == Events.Resource.Update)
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

            resourceTracker.AddDate(r.Published);
        }

        private async Task UpdateFeed(Feed f)
        {
            var c = new UpdateFeedListCommand(f);
            await c.ApplyAsync(feedStorageService);
        }
    }
}
