using Dapr.Client;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace megaphone.feeds.Controllers
{
    [ApiController]
    [Route("/api/feeds")]
    public class FeedsController : ControllerBase
    {
        private FeedStorageService feedStorageService;

        public FeedsController([FromServices] DaprClient daprClient)
        {
            feedStorageService = new FeedStorageService(daprClient);
        }

        [Route("")]
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            return new JsonResult(entry.Value);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            var q = new GetFeedListQuery();
            var entry = await q.ExecuteAsync(feedStorageService);

            var feed = entry.Value.FirstOrDefault(i => i.Id == id);

            return new JsonResult(feed);
        }
    }
}
