using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Feeds
{
    public class FeedService : IFeedService
    {
        bool initializationRequired = true;

        private DateTimeOffset lastPersisted;
        private DateTimeOffset lastUpdated;

        public DateTimeOffset LastUpdated => lastUpdated;

        private readonly IFeedStorageService feedStorageService;
        
        private readonly IClock clock;
        
        private Timer timer;

        public FeedService(IFeedStorageService feedStorageService, IClock clock)
        {
            this.feedStorageService = feedStorageService;
            this.clock = clock;

            SetupTimer();
        }

        private void SetupTimer()
        {
            timer = new Timer(async state =>
            {
                if (lastPersisted < lastUpdated)
                {
                    await SetAsync(new StorageEntry<List<Feed>>() { Value = feeds.Values.ToList() });
                }                   
             }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
        }

        ConcurrentDictionary<string, Feed> feeds = new();

        public async Task<List<Feed>> GetAsync()
        {
            if (initializationRequired)
                await LoadFromStorage();

            return feeds.Values.ToList();
        }

        private async Task LoadFromStorage()
        {
            var entity = await feedStorageService.GetAsync("feed", "list.json");
            if (entity.HasValue)
                entity.Value.ForEach(f => feeds.AddOrUpdate(f.Id, f, (key, oldValue) => f));
            
            Updated();

            initializationRequired = false;
        }

        private void Updated()
        {
            lastUpdated = clock.Now;
        }

        private async Task SetAsync(StorageEntry<List<Feed>> content)
        {
            await feedStorageService.SetAsync("feed", "list.json", content);
        }

        public void Update(Feed f)
        {
            feeds.AddOrUpdate(f.Id, (key) => f, (key, v) =>
            {
                v.LastCrawled = f.LastCrawled;
                v.LastHttpStatus = f.LastHttpStatus;
                v.Display = f.Display;

                return v;
            });

            Updated();
        }

        public void Delete(string id)
        {
            feeds.Remove(id, out var f);
        }
    }
}