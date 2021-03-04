using System.Threading.Tasks;
using System.Collections.Generic;
using Megaphone.Feeds.Services;
using Megaphone.Feeds.Models;
using Feeds.API.Commands;
using Megaphone.Standard.Commands;
using Megaphone.Standard.Services;
using Megaphone.Feeds.Queries;
using System.Diagnostics;
using System;

namespace Megaphone.Feeds.Commands
{
    internal class UpsertResourceListCommand : ICommand<IPartionedStorageService<StorageEntry<List<Resource>>>>
    {
        readonly Resource resource;

        public UpsertResourceListCommand(Resource resource)
        {
            this.resource = resource;
        }

        public async Task ApplyAsync(IPartionedStorageService<StorageEntry<List<Resource>>> model)
        {
            var q = new GetResourceListQuery(resource.Published);
            var entry = await q.ExecuteAsync(model);

            if (!entry.HasValue)
            {
                entry.Value = new List<Resource> { resource };

                if (Debugger.IsAttached)
                    Console.WriteLine($"resource update : \"{resource.Display}\" ({resource.Published.ToString("s")})");
               
            }
            else
            {
                var i = entry.Value.Find(i => i.Id == resource.Id);
                if (IsNotDefault(i))
                {
                    i.Display = resource.Display;
                    i.IsActive = resource.IsActive;
                    i.Updated = resource.Updated;
                    i.Published = resource.Published;

                    if (Debugger.IsAttached)
                        Console.WriteLine($"resource update : \"{i.Display}\" ({i.Published.ToString("s")})");
                }
                else
                {
                    entry.Value.Add(resource);

                    if (Debugger.IsAttached)
                        Console.WriteLine($"resource update : \"{resource.Display}\" ({resource.Published.ToString("s")})");
                }                
            }

            var c = new PersistResourceListCommand(resource.Published, entry);
            await c.ApplyAsync(model);           
        }

        static bool IsNotDefault(Resource r)
        {
            return r != null && !string.IsNullOrEmpty(r.Id);
        }
    }
}