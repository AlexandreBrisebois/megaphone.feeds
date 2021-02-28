using System;
using System.Text.Json.Serialization;

namespace Megaphone.Feeds.Models
{
    public class Feed
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("display")]
        public string Display { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("added")]
        public DateTimeOffset Added { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("lastCraweled")]
        public DateTimeOffset LastCrawled { get; set; } = DateTimeOffset.MinValue;

        [JsonPropertyName("lastHttpStatus")]
        public int? LastHttpStatus { get; set; } = null;
    }
}
