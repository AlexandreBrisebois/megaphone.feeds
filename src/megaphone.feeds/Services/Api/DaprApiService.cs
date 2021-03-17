using Dapr.Client;
using Megaphone.Feeds.Models.Views;
using System.Net.Http;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Api
{
    public class DaprApiService : IApiService
    {
        private readonly DaprClient daprClient;

        public DaprApiService(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task PublishAsync(ResourceListView view)
        {
            await daprClient.InvokeMethodAsync(HttpMethod.Post, "api", "api/resources", view);
        }

        public async Task PublishAsync(FeedListView view)
        {
            await daprClient.InvokeMethodAsync(HttpMethod.Put, "api", "api/feeds", view);
        }
    }
}