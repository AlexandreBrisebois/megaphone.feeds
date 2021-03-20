using Megaphone.Feeds.Mocks;
using Megaphone.Feeds.Services.Api;
using Megaphone.Feeds.Services.Crawler;
using Megaphone.Feeds.Services.Feeds;
using Megaphone.Feeds.Services.Resources;
using Megaphone.Feeds.Services.Storage;
using Megaphone.Standard.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;

namespace megaphone.feeds
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterServices(services);

            services.AddControllers().AddDapr();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Megaphone Feeds", Version = "v1" });
            });

            string key = Environment.GetEnvironmentVariable("INSTRUMENTATION_KEY");
            if (!string.IsNullOrEmpty(key))
                services.AddApplicationInsightsTelemetry(key);
            else
                services.AddApplicationInsightsTelemetry();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            if (Debugger.IsAttached)
            {
                services.AddSingleton<ICrawlerService, MockCrawlerService>();
                services.AddSingleton<IApiService, MockApiService>();

                services.AddSingleton<IFeedStorageService, InMemoryFeedStorageService>();
                services.AddSingleton<IResourceStorageService, InMemoryResourceStorageService>();
            }
            else
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USE-VOLUME-STORAGE")))
                {
                    services.AddSingleton<IFeedStorageService, DaprFeedStorageService>();
                    services.AddSingleton<IResourceStorageService, DaprResourceStorageService>();
                }
                else
                {
                    services.AddSingleton<IFeedStorageService>(new FileSystemFeedStorageService());
                    services.AddSingleton<IResourceStorageService>(new FileSystemResourceStorageService());
                }
                services.AddSingleton<ICrawlerService, DaprCrawlerService>();
                services.AddSingleton<IApiService, DaprApiService>();
            }

            services.AddSingleton<IClock, UtcClock>();

            services.AddSingleton<IResourceService, ResourceService>();
            services.AddSingleton<IFeedService, FeedService>();

            services.AddSingleton(new ResourceListChangeTracker());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Megaphone Feeds v1"));
            }

            app.UseCloudEvents();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
        }
    }
}
