using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Mocks
{
    public class InMemoryFeedStorageService : IFeedStorageService
    {
        InMemoryStorageService<StorageEntry<List<Feed>>> backingStore = new();
        public async Task<StorageEntry<List<Feed>>> GetAsync(string partitionKey, string contentKey)
        {
            return await backingStore.GetAsync($"{partitionKey}/{contentKey}");
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Feed>> content)
        {
            await backingStore.SetAsync($"{partitionKey}/{contentKey}", content);
        }
    }
}