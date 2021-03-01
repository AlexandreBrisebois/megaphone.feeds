using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Megaphone.Feeds.Models.Views
{
    public class FeedListView
    {
        [JsonPropertyName("updated")]
        public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("feeds")]
        public List<FeedView> Feeds { get; set; } = new List<FeedView>();
    }
}
