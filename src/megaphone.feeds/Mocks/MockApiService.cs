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

        public ConcurrentQueue<FeedListView> FeedLists { get; init; } = new ConcurrentQueue<FeedListView>();
        public ConcurrentQueue<ResourceListView> ResourceLists { get; init; } = new ConcurrentQueue<ResourceListView>();

        public Task PublishAsync(ResourceListView view)
        {
            ResourceLists.Enqueue(view);

            Console.WriteLine("published resource list =>" + view.Date.ToShortDateString());

            return Task.CompletedTask;
        }

        public Task PublishAsync(FeedListView view)
        {
            FeedLists.Enqueue(view);

            Console.WriteLine("published feed list =>" + view.Updated.DateTime.ToShortDateString());

            return Task.CompletedTask;
        }
    }
}