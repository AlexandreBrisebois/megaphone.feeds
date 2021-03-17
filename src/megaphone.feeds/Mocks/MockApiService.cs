using Megaphone.Feeds.Models.Views;
using Megaphone.Feeds.Services.Api;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Mocks
{
    public class MockApiService : IApiService
    {
        public static readonly MockCrawlerService Instance = new();

        public ConcurrentBag<FeedListView> FeedLists { get; init; } = new ConcurrentBag<FeedListView>();
        public ConcurrentBag<ResourceListView> ResourceLists { get; init; } = new ConcurrentBag<ResourceListView>();

        public Task PublishAsync(ResourceListView view)
        {
            ResourceLists.Add(view);

            Console.WriteLine("published resource list =>" + view.Date.ToShortDateString());

            return Task.CompletedTask;
        }

        public Task PublishAsync(FeedListView view)
        {
            FeedLists.Add(view);

            Console.WriteLine("published feed list =>" + view.Updated.DateTime.ToShortDateString());

            return Task.CompletedTask;
        }
    }
}