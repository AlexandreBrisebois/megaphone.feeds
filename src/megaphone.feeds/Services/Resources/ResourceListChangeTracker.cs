using System;
using System.Collections.Concurrent;

namespace Megaphone.Feeds.Services.Resources
{
    public class ResourceListChangeTracker
    {
        private ConcurrentBag<DateTime> resourceListViews = new ConcurrentBag<DateTime>();

        public void AddDate(DateTime date)
        {
            resourceListViews.Add(date);
        }

        public bool TryRemoveDate(out DateTime date)
        {
            var isSucess = resourceListViews.TryTake(out var d);
            date = d;

            return isSucess;
        }

    }
}