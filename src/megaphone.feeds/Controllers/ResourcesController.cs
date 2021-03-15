using Dapr.Client;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Controllers
{
    [ApiController]
    [Route("/api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly TelemetryClient telemetryClient;

        public ResourcesController(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        [HttpGet]
        [Route("{year}/{month}/{day}")]
        [ProducesResponseType(typeof(List<Resource>),(int)HttpStatusCode.OK)]
        public async Task<List<Resource>> GetAsync([FromServices] DaprClient daprClient, int year, int month, int day)
        {
            var storage = new ResourceStorageService(daprClient);

            var q = new GetResourceListQuery(new DateTime(year, month, day));
            var entry = await q.ExecuteAsync(storage);

            return entry.Value ?? new List<Resource>();
        }
    }
}
