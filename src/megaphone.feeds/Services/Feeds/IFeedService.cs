using Megaphone.Feeds.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Feeds
{
    public interface IFeedService
    {
        Task<List<Feed>> GetAsync();
        void Update(Feed f);
        void Delete(string id);
    }
}