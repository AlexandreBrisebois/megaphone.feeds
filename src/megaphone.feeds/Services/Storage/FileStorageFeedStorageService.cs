using Megaphone.Feeds.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Storage
{
    public class FileStorageFeedStorageService : IFeedStorageService
    {
        private string path;

        public FileStorageFeedStorageService()
        {
            path = Environment.GetEnvironmentVariable("DATA_PATH");
        }

        public async Task<StorageEntry<List<Feed>>> GetAsync(string partitionKey, string contentKey)
        {
            string filePath = $"{path}/{partitionKey}/{contentKey}";

            if (File.Exists(filePath))
            {
                using var stream = File.OpenRead(filePath);
                using var reader = new StreamReader(stream);

                var content = await reader.ReadToEndAsync();
                var obj = JsonSerializer.Deserialize<StorageEntry<List<Feed>>>(content);
                return obj;
            }

            return new StorageEntry<List<Feed>>();
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Feed>> content)
        {
            string filePath = $"{path}/{partitionKey}/{contentKey}";

            using var stream = File.OpenWrite(filePath);
            using var writer = new StreamWriter(stream);

            await writer.WriteAsync(JsonSerializer.Serialize(content));
            await writer.FlushAsync();
        }
    }
}
