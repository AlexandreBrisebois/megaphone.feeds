using Feeds.API.Commands;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Feeds;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Hosted
{
    public class FeedUpdaterService : IHostedService, IDisposable
    {
        private readonly IFeedService feedService;
        private readonly ICrawlerService crawlerService;

        private readonly TelemetryClient telemetryClient;

        private Timer timer;

        public FeedUpdaterService([FromServices] TelemetryClient telemetryClient,
                                  [FromServices] ICrawlerService crawlerService,
                                  [FromServices] IFeedService feedService)
        {
            this.telemetryClient = telemetryClient;
            this.crawlerService = crawlerService;
            this.feedService = feedService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async state =>
            {
                telemetryClient.TrackEvent("feed-updater-service-cycle-start");

                var tasks = new[] { TrySendFeedCrawlRequests() };
                await Task.WhenAll(tasks);

                telemetryClient.TrackEvent("feed-updater-service-cycle-end");
            },
                      null,
                      TimeSpan.Zero,
                      TimeSpan.FromMinutes(20));

            telemetryClient.TrackEvent("started-feed-updater-service");

            return Task.CompletedTask;
        }

        private async Task TrySendFeedCrawlRequests()
        {
            try
            {
                var entry = await feedService.GetAsync();

                foreach (var f in entry.Value ?? new List<Feed>())
                {
                    var c = new SendCrawlRequestCommand(f);
                    await c.ApplyAsync(crawlerService);

                    if (Debugger.IsAttached)
                        Console.WriteLine($"-> | sent crawl request : \"{f.Display}\" : {f.Url}");
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

            telemetryClient.TrackEvent("stopped-feed-updater-service");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}