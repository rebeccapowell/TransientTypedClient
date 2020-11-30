using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DemoConsoleApp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DemoConsoleApp
{
    class Program
    {
        static void Main(string[] args) => Run().GetAwaiter().GetResult();

        public static async Task Run()
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IGitHubService, GitHubService>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "My User Agent Example");
                    client.DefaultRequestHeaders.Add("Instance", Guid.NewGuid().ToString());
                    client.BaseAddress = new Uri("https://api.github.com/");
                });

            services.AddSingleton<IGitHubServiceFactory, GitHubServiceFactory>();

            var p = services.BuildServiceProvider();
            var f = p.GetRequiredService<IServiceScopeFactory>();

            var repos = new List<Repository>();
            var stopWatch = new Stopwatch();

            // request 1
            stopWatch.Start();
            using (var serviceScope = f.CreateScope())
            {
                var provider = serviceScope.ServiceProvider;
                var gitHubServiceFactory = provider.GetService<IGitHubServiceFactory>();
                var gitHubService = gitHubServiceFactory.GetService();

                repos.AddRange(await gitHubService.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");

                stopWatch.Start();
                repos.AddRange(await gitHubService.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // request 2
            stopWatch.Start();
            using (var serviceScope = f.CreateScope())
            {
                var provider = serviceScope.ServiceProvider;
                var gitHubServiceFactory = provider.GetService<IGitHubServiceFactory>();
                var gitHubService = gitHubServiceFactory.GetService();

                repos.AddRange(await gitHubService.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // request 3
            stopWatch.Start();
            using (var serviceScope = f.CreateScope())
            {
                var provider = serviceScope.ServiceProvider;
                var gitHubServiceFactory = provider.GetService<IGitHubServiceFactory>();
                var gitHubService = gitHubServiceFactory.GetService();

                repos.AddRange(await gitHubService.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }
            
            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}
