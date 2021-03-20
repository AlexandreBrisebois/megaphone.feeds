using Megaphone.Feeds.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Storage
{
    public class FileSystemResourceStorageService : IResourceStorageService
    {
        private string path;

        public FileSystemResourceStorageService()
        {
            path = Environment.GetEnvironmentVariable("DATA_PATH");
        }

        public async Task<StorageEntry<List<Resource>>> GetAsync(string partitionKey, string contentKey)
        {
            string filePath = $"{path}/resources/{partitionKey}/{contentKey}";

            if (File.Exists(filePath))
            {
                using var stream = File.OpenRead(filePath);
                using var reader = new StreamReader(stream);

                var content = await reader.ReadToEndAsync();
                var obj = JsonSerializer.Deserialize<StorageEntry<List<Resource>>>(content);
                return obj;
            }

            return new StorageEntry<List<Resource>>();
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Resource>> content)
        {
            string filePath = $"{path}/resources/{partitionKey}/{contentKey}";

            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            using var stream = fileInfo.Create();
            using var writer = new StreamWriter(stream);

            await writer.WriteAsync(JsonSerializer.Serialize(content));
            await writer.FlushAsync();
        }
    }
}
