using Feeds.API.Commands;
using Megaphone.Feeds.Services.Crawler;
using Megaphone.Feeds.Services.Feeds;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Hosted
{
    public class FeedUpdaterService : BackgroundService
    {
        private readonly TelemetryClient telemetryClient;
        private readonly IFeedService feedService;
        private readonly ICrawlerService crawlerService;
        private Task taskLoop;

        public FeedUpdaterService([FromServices] TelemetryClient telemetryClient,
                                  [FromServices] ICrawlerService crawlerService,
                                  [FromServices] IFeedService feedService)
        {
            this.telemetryClient = telemetryClient;
            this.crawlerService = crawlerService;
            this.feedService = feedService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            telemetryClient.TrackEvent("FeedUpdater: Execute Called");

            taskLoop = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    telemetryClient.TrackEvent("FeedUpdater: Try Send Feed Crawl Requests");
                    await TrySendFeedCrawlRequests();

                    await Task.Delay(TimeSpan.FromMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("SCHEDULE-CRAWL-INTERVAL"))));
                }
            });

            return taskLoop;
        }

        private async Task TrySendFeedCrawlRequests()
        {
            try
            {
                var entry = await feedService.GetAsync();

                foreach (var f in entry)
                {
                    try
                    {
                        var c = new SendCrawlRequestCommand(f);
                        await c.ApplyAsync(crawlerService);
                    }
                    catch (Exception ex)
                    {
                        telemetryClient.TrackException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
            }
        }
    }    
}