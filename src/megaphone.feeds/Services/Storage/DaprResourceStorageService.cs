using Dapr.Client;
using Megaphone.Feeds.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Services.Storage
{
    public class DaprResourceStorageService : IResourceStorageService
    {
        const string STATE_STORE = "feed-state";

        private readonly DaprClient client;
        private readonly Dictionary<string, string> trackedEtags = new Dictionary<string, string>();

        public DaprResourceStorageService(DaprClient client)
        {
            this.client = client;
        }
        public async Task<StorageEntry<List<Resource>>> GetAsync(string partitionKey, string contentKey)
        {
            string key = $"{partitionKey}/{contentKey}";
            var (value, etag) = await client.GetStateAndETagAsync<StorageEntry<List<Resource>>>(STATE_STORE, key);
            trackedEtags[key] = etag;

            return value ?? new StorageEntry<List<Resource>>();
        }

        public async Task SetAsync(string partitionKey, string contentKey, StorageEntry<List<Resource>> content)
        {
            string key = $"{partitionKey}/{contentKey}";

            content.Updated = DateTimeOffset.UtcNow;

            if (trackedEtags.ContainsKey(key) && !string.IsNullOrEmpty(trackedEtags[key]))
            {
                var stateSaved = await client.TrySaveStateAsync(STATE_STORE, key, content, trackedEtags[key]);
                if (stateSaved)
                    return;
                throw new Exception($"failed to save state for {key}");
            }
            else
            {
                await client.SaveStateAsync(STATE_STORE, key, content);
            }
        }
    }
}
