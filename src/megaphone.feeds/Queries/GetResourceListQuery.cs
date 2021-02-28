using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Queries;
using Megaphone.Standard.Services;

namespace Megaphone.Feeds.Queries
{
    class GetResourceListQuery : IQuery<IPartionedStorageService<StorageEntry<List<Resource>>>, StorageEntry<List<Resource>>>
    {
        const string CONTENT_KEY = "resources.json";
        string partitionKey = string.Empty;

        public GetResourceListQuery(DateTime date)
        {
            partitionKey = $"{date.Year}/{date.Month}/{date.Day}";
        }

        public async Task<StorageEntry<List<Resource>>> ExecuteAsync(IPartionedStorageService<StorageEntry<List<Resource>>> model)
        {
            return await model.GetAsync(partitionKey, CONTENT_KEY);
        }
    }
}