using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Mocks
{
    public class InMemoryResourceStorageService : IResourceStorageService
    {
        InMemoryStorageService<StorageEntry<List<Resource>>> backingStore = new();
        public async Task<StorageEntry<List<Resource>>> GetAsync(string partitionKey, string contentKey)
        {
            try
            {
                return await backingStore.GetAsync($"{partitionKey}/{contentKey}");
            }catch
            {
                return new StorageEntry<List<Resource>>();
            }
            
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Resource>> content)
        {
            await backingStore.SetAsync($"{partitionKey}/{contentKey}", content);
        }
    }
}