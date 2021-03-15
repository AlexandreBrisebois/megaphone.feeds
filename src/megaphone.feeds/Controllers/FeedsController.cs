using Dapr.Client;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace megaphone.feeds.Controllers
{
    [ApiController]
    [Route("/api/feeds")]
    public class FeedsController : ControllerBase
    {
        private readonly TelemetryClient telemetryClient;
        private FeedStorageService feedStorageService;

        public FeedsController([FromServices] DaprClient daprClient, TelemetryClient telemetryClient)
        {
            feedStorageService = new FeedStorageService(daprClient);
            this.telemetryClient = telemetryClient;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Feed>), (int)HttpStatusCode.OK)]
        public async Task<List<Feed>> GetAsync()
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            return entry.Value;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Feed), (int)HttpStatusCode.OK)]
        public async Task<Feed> GetAsync(string id)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            var feed = entry.Value.FirstOrDefault(i => i.Id == id);

            return feed;
        }
    }
}
