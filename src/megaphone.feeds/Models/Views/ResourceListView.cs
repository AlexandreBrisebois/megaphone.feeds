using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Megaphone.Feeds.Models.Views
{
    public class ResourceListView
    {
        [JsonPropertyName("updated")]
        public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        [JsonPropertyName("resources")]
        public List<ResourceView> Resources { get; set; } = new List<ResourceView>();
    }
}
