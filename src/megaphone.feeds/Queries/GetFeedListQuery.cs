using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Queries
{
    class GetFeedListQuery : IQuery<IFeedService, StorageEntry<List<Feed>>>
    {
        public async Task<StorageEntry<List<Feed>>> ExecuteAsync(IFeedService model)
        {
            var entry = await model.GetAsync("feed", "list.json");
            return entry;
        }
    }
}