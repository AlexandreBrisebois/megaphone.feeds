using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services.Resources;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Feeds.API.Commands
{
    internal class PersistResourceListCommand : ICommand<IResourceService>
    {
        const string CONTENT_KEY = "resources.json";

        readonly string partitionKey;
        readonly StorageEntry<List<Resource>> entry;

        public PersistResourceListCommand(DateTime date, StorageEntry<List<Resource>> entry)
        {
            partitionKey = $"{date.Year}/{date.Month}/{date.Day}";
            this.entry = entry;
        }

        public async Task ApplyAsync(IResourceService model)
        {
            await model.SetAsync(partitionKey, CONTENT_KEY, entry);
        }
    }
}