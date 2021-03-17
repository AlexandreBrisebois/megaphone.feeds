using Megaphone.Feeds.Models.Views;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public interface IApiService
    {
        Task PublishAsync(ResourceListView view);
        Task PublishAsync(FeedListView view);
    }
}