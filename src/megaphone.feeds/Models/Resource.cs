using System;
using System.Text.Json.Serialization;

namespace Megaphone.Feeds.Models
{
    public class Resource
    {
        [JsonPropertyName("updated")]
        public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("published")]
        public DateTime Published { get; set; } = DateTime.UtcNow.Date;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("display")]
        public string Display { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}