using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public interface IFeedService
    {
        Task<StorageEntry<List<Feed>>> GetAsync(string partitionKey, string contentKey);
        Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Feed>> content);
    }
}