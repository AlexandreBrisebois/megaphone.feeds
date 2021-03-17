using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Feeds;
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
        private IFeedService feedService;

        public FeedsController([FromServices] IFeedService feedService)
        {
            this.feedService = feedService;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Feed>), (int)HttpStatusCode.OK)]
        public async Task<List<Feed>> GetAsync()
        {
            var entry = await feedService.GetAsync();

            return entry.Value;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Feed), (int)HttpStatusCode.OK)]
        public async Task<Feed> GetAsync(string id)
        {
            var entry = await feedService.GetAsync();

            var feed = entry.Value.FirstOrDefault(i => i.Id == id);

            return feed;
        }
    }
}
