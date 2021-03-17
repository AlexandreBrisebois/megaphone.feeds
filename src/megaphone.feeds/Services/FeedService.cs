using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{

    public class FeedService : IFeedService
    {
        private readonly IFeedStorageService feedStorageService;

        public FeedService(IFeedStorageService feedStorageService)
        {
            this.feedStorageService = feedStorageService;
        }

        public async Task<StorageEntry<List<Feed>>> GetAsync(string partitionKey, string contentKey)
        {
            return await feedStorageService.GetAsync(partitionKey, contentKey);
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Feed>> content)
        {
            await feedStorageService.SetAsync(partitionKey, contentKey, content);
        }
    }
}