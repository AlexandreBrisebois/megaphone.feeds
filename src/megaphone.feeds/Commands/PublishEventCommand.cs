using System.Threading.Tasks;
using Megaphone.Standard.Commands;
using Dapr.Client;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace Feeds.API.Commands
{

    class PublishEventCommand : ICommand<DaprClient>
    {
        private readonly object content;
        private readonly string pubsubName;
        private readonly string topic;

        public PublishEventCommand(object content, string pubsubName, string topic)
        {
            this.content = content;
            this.pubsubName = pubsubName;
            this.topic = topic;
        }

        public async Task ApplyAsync(DaprClient model)
        {
            await model.PublishEventAsync(pubsubName, topic, content);

            if (Debugger.IsAttached)
                Console.WriteLine($"published event : \"{JsonSerializer.Serialize(content)}\"");
        }
    }
}