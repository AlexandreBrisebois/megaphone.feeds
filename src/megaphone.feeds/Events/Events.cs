namespace Megaphone.Feeds.Events
{
    internal static class Events
    {
        internal static class Feed
        {
            internal static readonly string Add = "add-feed";
            internal static readonly string Delete = "delete-feed";
            internal static readonly string UpdateFeedList = "updated-feed-list";
            internal static readonly string SentCrawlRequest = "sent-crawl-request";
        }

        internal static class Resource
        {
            internal static readonly string Update = "update-resource";
        }
    }
}
