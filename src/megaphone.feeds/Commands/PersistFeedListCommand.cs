using Megaphone.Feeds.Models;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Feeds.API.Commands
{
    class PersistFeedListCommand : ICommand<IFeedService>
    {
        readonly StorageEntry<List<Feed>> entry;

        public PersistFeedListCommand(StorageEntry<List<Feed>> entry)
        {
            this.entry = entry;
        }

        public async Task ApplyAsync(IFeedService model)
        {
            await model.SetAsync("feed", "list.json", entry);
        }
    }
}