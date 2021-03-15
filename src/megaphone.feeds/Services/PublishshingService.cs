using Dapr.Client;
using Megaphone.Feeds.Models.Views;
using Megaphone.Feeds.Queries;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public class PublishshingService : IHostedService, IDisposable
    {
        DateTimeOffset lastUpdated = DateTimeOffset.MinValue;

        private readonly FeedStorageService feedStorageService;
        private readonly ResourceStorageService resourceStorageService;

        private readonly ResourceListChangeTracker resourceTracker;
        private readonly TelemetryClient telemetryClient;

        private readonly DaprClient daprClient;

        private Timer timer;

        public PublishshingService([FromServices] DaprClient daprClient,
                                   [FromServices] ResourceListChangeTracker resourceTracker,
                                   TelemetryClient telemetryClient)
        {
            feedStorageService = new FeedStorageService(daprClient);
            resourceStorageService = new ResourceStorageService(daprClient);

            this.daprClient = daprClient;
            this.resourceTracker = resourceTracker;
            this.telemetryClient = telemetryClient;
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
                      TimeSpan.FromSeconds(5));

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
                    var entry = await q.ExecuteAsync(resourceStorageService);

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

                    await daprClient.InvokeMethodAsync(HttpMethod.Post, "api", "api/resources", view);

                    telemetryClient.TrackEvent("publish-resources-to-api-service", new Dictionary<string, string> { { "date", d.ToShortDateString() } });

                    if (Debugger.IsAttached)
                        Console.WriteLine($"-> | published ({d.ToShortDateString()}) resource to API service");
                }
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
            }
        }

        private List<DateTime> GetDates()
        {
            List<DateTime> dates = new List<DateTime>();
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
                var q = new GetFeedListQuery();
                var entry = await q.ExecuteAsync(feedStorageService);

                if (lastUpdated != entry?.Updated)
                {
                    if (entry.HasValue)
                    {
                        var view = new FeedListView
                        {
                            Feeds = entry.Value.Select(f => new FeedView { Display = f.Display, Url = f.Url, Id = f.Id }).ToList()
                        };

                        await daprClient.InvokeMethodAsync(HttpMethod.Put, "api", "api/feeds", view);

                        telemetryClient.TrackEvent("publish-feeds-to-api-service");

                        if (Debugger.IsAttached)
                            Console.WriteLine($"-> | published Feeds to API service");

                        lastUpdated = entry.Updated;
                    }
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