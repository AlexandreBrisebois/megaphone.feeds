using Megaphone.Feeds.Models.Views;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services.Api;
using Megaphone.Feeds.Services.Feeds;
using Megaphone.Feeds.Services.Resources;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Hosted
{
    public class PublishshingService : IHostedService, IDisposable
    {
        DateTimeOffset lastUpdated = DateTimeOffset.MinValue;

        private readonly TelemetryClient telemetryClient;
        private readonly ResourceListChangeTracker resourceTracker;
        private readonly IFeedService feedService;
        private readonly IResourceService resourceService;
        private readonly IApiService apiService;

        private Timer timer;

        public PublishshingService([FromServices] TelemetryClient telemetryClient,
                                   [FromServices] ResourceListChangeTracker resourceTracker,
                                   [FromServices] IApiService apiService,
                                   [FromServices] IFeedService feedService,
                                   [FromServices] IResourceService resourceService)
        {
            this.telemetryClient = telemetryClient;
            this.resourceTracker = resourceTracker;
            this.apiService = apiService;
            this.feedService = feedService;
            this.resourceService = resourceService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async state =>
            {
                telemetryClient.TrackEvent("publishing-service-cycle-start");

                var tasks = new[] { TryPushFeedUpdates(), TryPushResourceListUpdates() };
                await Task.WhenAll(tasks);

                telemetryClient.TrackEvent("publishing-service-cycle-end");
            },
                      null,
                      TimeSpan.Zero,
                      TimeSpan.FromSeconds(30));

            telemetryClient.TrackEvent("started-publishing-service");

            return Task.CompletedTask;
        }

        private async Task TryPushResourceListUpdates()
        {
            try
            {
                List<DateTime> dates = GetDates();

                foreach (var d in dates)
                {
                    var q = new GetResourceListQuery(d);
                    var entry = await q.ExecuteAsync(resourceService);

                    var view = new ResourceListView
                    {
                        Date = d,
                        Resources = entry.Value.Select(r => new ResourceView
                        {
                            Display = r.Display,
                            Url = r.Url,
                            Id = r.Id
                        }).ToList()
                    };

                    await apiService.PublishAsync(view);

                    telemetryClient.TrackEvent("publish-resources-to-api-service", new Dictionary<string, string> { { "date", d.ToShortDateString() } });
                }
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
            }
        }

        private List<DateTime> GetDates()
        {
            List<DateTime> dates = new();
            while (resourceTracker.TryRemoveDate(out DateTime date))
            {
                dates.Add(date.Date);
            }

            dates = dates.Distinct().ToList();
            return dates;
        }

        private async Task TryPushFeedUpdates()
        {
            try
            {
                var entry = await feedService.GetAsync();

                if (lastUpdated != feedService.LastUpdated)
                {

                    var view = new FeedListView
                    {
                        Feeds = entry.Select(f => new FeedView { Display = f.Display, Url = f.Url, Id = f.Id }).ToList()
                    };

                    await apiService.PublishAsync(view);

                    telemetryClient.TrackEvent("publish-feeds-to-api-service");

                    lastUpdated = feedService.LastUpdated;
                }
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            telemetryClient.TrackEvent("stopped-publishing-service");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}