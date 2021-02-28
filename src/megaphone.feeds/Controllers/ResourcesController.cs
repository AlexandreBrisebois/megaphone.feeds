using Dapr.Client;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace megaphone.feeds.Controllers
{
    [ApiController]
    [Route("/api/resources")]
    public class ResourcesController : ControllerBase
    {
        [Route("{year}/{month}/{day}")]
        [HttpGet]
        public async Task<JsonResult> GetAsync([FromServices] DaprClient daprClient, int year, int month, int day)
        {
            var storage = new ResourceStorageService(daprClient);

            var q = new GetResourceListQuery(new DateTime(year, month, day));
            var entry = await q.ExecuteAsync(storage);

            return new JsonResult(entry.Value ?? new List<Resource>());
        }
    }
}
