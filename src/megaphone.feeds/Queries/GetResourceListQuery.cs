using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Queries
{
    class GetResourceListQuery : IQuery<IResourceService, StorageEntry<List<Resource>>>
    {
        const string CONTENT_KEY = "resources.json";
        string partitionKey = string.Empty;

        public GetResourceListQuery(DateTime date)
        {
            partitionKey = $"{date.Year}/{date.Month}/{date.Day}";
        }

        public async Task<StorageEntry<List<Resource>>> ExecuteAsync(IResourceService model)
        {
            return await model.GetAsync(partitionKey, CONTENT_KEY);
        }
    }
}