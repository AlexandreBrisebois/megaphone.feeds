using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Commands;
using Megaphone.Standard.Services;

namespace Feeds.API.Commands
{
    class PersistFeedListCommand : ICommand<IPartionedStorageService<StorageEntry<List<Feed>>>>
    {
        readonly StorageEntry<List<Feed>> entry;

        public PersistFeedListCommand(StorageEntry<List<Feed>> entry)
        {
            this.entry = entry;
        }

        public async Task ApplyAsync(IPartionedStorageService<StorageEntry<List<Feed>>> model)
        {
            await model.SetAsync("feed", "list.json", entry);

            if (Debugger.IsAttached)
                Console.WriteLine($"feeds updatd : \"list.json");
        }
    }
}