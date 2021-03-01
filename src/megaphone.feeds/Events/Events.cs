namespace Megaphone.Feeds.Events
{

    public static class Events
    {
        public static class Feed
        {
            public static readonly string Add = "add-feed";
            public static readonly string Delete = "delete-feed";
        }

        public static class Resource
        {
            public static readonly string Update = "update-resource";
        }
    }
}
