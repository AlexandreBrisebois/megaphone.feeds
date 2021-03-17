using Feeds.API.Commands;
using Megaphone.Feeds.Models;
using Megaphone.Feeds.Queries;
using Megaphone.Feeds.Services;
using Megaphone.Standard.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Megaphone.Feeds.Commands
{
    internal class UpsertResourceListCommand : ICommand<IResourceService>
    {
        readonly Resource resource;

        public UpsertResourceListCommand(Resource resource)
        {
            this.resource = resource;
        }

        public async Task ApplyAsync(IResourceService model)
        {
            var q = new GetResourceListQuery(resource.Published);
            var entry = await q.ExecuteAsync(model);

            if (!entry.HasValue)
            {
                entry.Value = new List<Resource> { resource };

                if (Debugger.IsAttached)
                    Console.WriteLine($"[] | resource update : \"{resource.Display}\" ({resource.Published.ToString("s")})");

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
                        Console.WriteLine($"[] | resource update : \"{i.Display}\" ({i.Published.ToString("s")})");
                }
                else
                {
                    entry.Value.Add(resource);

                    if (Debugger.IsAttached)
                        Console.WriteLine($"[] | resource update : \"{resource.Display}\" ({resource.Published.ToString("s")})");
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