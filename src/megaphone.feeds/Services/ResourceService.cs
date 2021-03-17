using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceStorageService resourceService;

        public ResourceService(IResourceStorageService resourceService)
        {
            this.resourceService = resourceService;
        }

        public async Task<StorageEntry<List<Resource>>> GetAsync(string partitionKey, string contentKey)
        {
            return await resourceService.GetAsync(partitionKey, contentKey);
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Resource>> content)
        {
            await resourceService.SetAsync(partitionKey, contentKey, content);
        }
    }
}