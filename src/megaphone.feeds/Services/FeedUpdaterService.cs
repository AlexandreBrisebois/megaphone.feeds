using Dapr.Client;
using Feeds.API.Commands;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public class FeedUpdaterService : IHostedService, IDisposable
    {
        private readonly FeedStorageService feedStorageService;

        private readonly DaprClient daprClient;

        private Timer timer;

        public FeedUpdaterService([FromServices] DaprClient daprClient)
        {
            feedStorageService = new FeedStorageService(daprClient);

            this.daprClient = daprClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async state =>
            {
                var tasks = new[] { TrySendFeedCrawlRequests() };
                await Task.WhenAll(tasks);
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

            return Task.CompletedTask;
        }

        private async Task TrySendFeedCrawlRequests()
        {
            try
            {
                var q = new GetFeedListQuery();
                var entry = await q.ExecuteAsync(feedStorageService);

                foreach (var f in entry.Value ?? new List<Feed>())
                {
                    var c = new SendCrawlRequestCommand(f);
                    await c.ApplyAsync(daprClient);

                    if (Debugger.IsAttached)
                        Console.WriteLine($"-> | sent crawl request : \"{f.Display}\" : {f.Url}");
                }
            }
            catch
            {
                // nothing
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}