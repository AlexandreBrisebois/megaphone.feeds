using Megaphone.Feeds.Models;
using Megaphone.Standard.Events;
using System;

namespace Megaphone.Feeds.Events
{
    public static class EventExtensions
    {
        public static bool TryConvertToFeed(this Event e, out Feed feed)
        {
            if (!e.Data.ContainsKey("resource") || e.Data["resource"]["type"] != ResourceType.Feed)
            {
                feed = null;
                return false;
            }

            feed = new Feed
            {
                Display = e.Data["resource"]["display"],
                Url = e.Data["resource"]["url"],
                Id = e.Data["resource"]["id"],
                LastHttpStatus = Convert.ToInt32(e.Metadata["status"]),
                LastCrawled = DateTimeOffset.Parse(e.Metadata["updated"])
            };

            return true;
        }

        public static bool TryConvertToResource(this Event e, out Resource resource)
        {
            if (!e.Data.ContainsKey("resource") || e.Data["resource"]["type"] != ResourceType.Page)
            {
                resource = null;
                return false;
            }

            resource = new Resource
            {
                Display = e.Data["resource"]["display"],
                Url = e.Data["resource"]["url"],
                Id = e.Data["resource"]["id"],
                IsActive = Convert.ToInt32(e.Metadata["status"]) == 200,
                Published = DateTime.Parse(e.Metadata["published"]),
                Updated = DateTime.Parse(e.Metadata["updated"])
            };

            return true;
        }
    }
}
