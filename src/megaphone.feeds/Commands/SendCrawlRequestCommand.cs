using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Commands;
using Megaphone.Standard.Messages;
using System.Threading.Tasks;

namespace Feeds.API.Commands
{
    internal class SendCrawlRequestCommand : ICommand<ICrawlerService>
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

        public SendCrawlRequestCommand(Feed feed)
        {
            message = MessageBuilder.NewCommand("crawl-request")
                                        .WithParameters("uri", feed.Url)
                                        .WithParameters("display", feed.Display)
                                        .Make();
        }

        public async Task ApplyAsync(ICrawlerService model)
        {
            await model.SendCrawlRequest(message);
        }
    }
}