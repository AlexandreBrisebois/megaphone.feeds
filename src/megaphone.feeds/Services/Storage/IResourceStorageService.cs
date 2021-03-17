using Megaphone.Feeds.Models;
using Megaphone.Standard.Services;
using System.Collections.Generic;

namespace Megaphone.Feeds.Services.Storage
{
    public interface IResourceStorageService : IPartionedStorageService<StorageEntry<List<Resource>>>
    {

    }
}