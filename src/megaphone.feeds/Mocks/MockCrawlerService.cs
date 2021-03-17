using Megaphone.Feeds.Services.Crawler;
using Megaphone.Standard.Messages;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Mocks
{
    public class MockCrawlerService : ICrawlerService
    {
        public static readonly MockCrawlerService Instance = new();

        public ConcurrentBag<CommandMessage> Messages { get; init; } = new ConcurrentBag<CommandMessage>();

        public Task SendCrawlRequest(CommandMessage message)
        {
            Messages.Add(message);

            Console.WriteLine("sent crawl request => "+JsonSerializer.Serialize(message));

            return Task.CompletedTask;
        }
    }
}