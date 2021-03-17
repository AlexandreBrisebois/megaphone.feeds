using Megaphone.Feeds.Models.Views;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Api
{
    public interface IApiService
    {
        Task PublishAsync(ResourceListView view);
        Task PublishAsync(FeedListView view);
    }
}