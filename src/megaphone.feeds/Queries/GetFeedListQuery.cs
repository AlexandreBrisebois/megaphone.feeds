using System.Collections.Generic;
using System.Threading.Tasks;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Queries;
using Megaphone.Standard.Services;

namespace Megaphone.Feeds.Queries
{
    class GetFeedListQuery : IQuery<IPartionedStorageService<StorageEntry<List<Feed>>>, StorageEntry<List<Feed>>>
    {
        public async Task<StorageEntry<List<Feed>>> ExecuteAsync(IPartionedStorageService<StorageEntry<List<Feed>>> model)
        {
            var entry = await model.GetAsync("feed", "list.json");
            return entry;
        }
    }
}