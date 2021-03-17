using Megaphone.Standard.Messages;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Crawler
{
    public interface ICrawlerService
    {
        Task SendCrawlRequest(CommandMessage message);
    }
}