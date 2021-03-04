using System.Threading.Tasks;
using Megaphone.Standard.Commands;
using Megaphone.Standard.Services;
using System.Collections.Generic;
using System;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Models;
using System.Diagnostics;

namespace Feeds.API.Commands
{
    internal class PersistResourceListCommand : ICommand<IPartionedStorageService<StorageEntry<List<Resource>>>>
    {
        const string CONTENT_KEY = "resources.json";

        readonly string partitionKey;
        readonly StorageEntry<List<Resource>> entry;

        public PersistResourceListCommand(DateTime date, StorageEntry<List<Resource>> entry)
        {
            partitionKey = $"{date.Year}/{date.Month}/{date.Day}";
            this.entry = entry;
        }

        public async Task ApplyAsync(IPartionedStorageService<StorageEntry<List<Resource>>> model)
        {
            await model.SetAsync(partitionKey, CONTENT_KEY, entry);

             if (Debugger.IsAttached)
                Console.WriteLine($"resources updatd : \"{CONTENT_KEY})");
        }
    }
}