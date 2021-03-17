using Dapr.Client;
using Megaphone.Standard.Messages;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services
{
    public class DaprCrawlerService : ICrawlerService
    {
        private readonly DaprClient daprClient;

        public DaprCrawlerService(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task SendCrawlRequest(CommandMessage message)
        {
            await daprClient.InvokeBindingAsync("crawl-requests", "create", message);
        }
    }
}