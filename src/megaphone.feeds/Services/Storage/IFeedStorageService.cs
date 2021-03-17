using Megaphone.Feeds.Models;
using Megaphone.Standard.Services;
using System.Collections.Generic;

namespace Megaphone.Feeds.Services.Storage
{

    public interface IFeedStorageService : IPartionedStorageService<StorageEntry<List<Feed>>>
    {
    }
}