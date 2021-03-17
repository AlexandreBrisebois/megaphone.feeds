using System;

namespace Megaphone.Feeds.Services.Storage
{
    public class StorageEntry<T> where T : new()
    {
        public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;

        public T Value { get; set; }

        public bool HasValue => Value != null;
    }
}
