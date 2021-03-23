using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Resources
{
    public interface IResourceService
    {
        Task<StorageEntry<List<Resource>>> GetAsync(string partitionKey, string contentKey);
        Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Resource>> content);
    }
}