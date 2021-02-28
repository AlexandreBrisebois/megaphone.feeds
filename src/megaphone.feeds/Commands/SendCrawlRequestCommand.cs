using System.Threading.Tasks;
using Megaphone.Standard.Commands;
using Dapr.Client;
using Megaphone.Standard.Messages;
using Megaphone.Feeds.Models;

namespace Feeds.API.Commands
{
    internal class SendCrawlRequestCommand : ICommand<DaprClient>
    {
        private readonly CommandMessage message;

        public SendCrawlRequestCommand(string uri)
        {
            message = MessageBuilder.NewCommand("crawl-request")
                                        .WithParameters("uri", uri)
                                        .Make();
        }

        public SendCrawlRequestCommand(Resource resource)
        {
            message = MessageBuilder.NewCommand("crawl-request")
                                        .WithParameters("uri", resource.Url)
                                        .Make();
        }

        public async Task ApplyAsync(DaprClient model)
        {
            await model.InvokeBindingAsync("crawl-requests", "create", message);
        }
    }
}