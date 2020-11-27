using System;
using System.Collections.Generic;
using DemoConsoleApp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DemoConsoleApp
{
    class Program
    {
        static void Main(string[] args)
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

            var gitHubServiceFactory = p.GetService<IGitHubServiceFactory>();
            var service = gitHubServiceFactory.GetService();

            do
            {
                while (!Console.KeyAvailable)
                {
                    var repos = new List<Repository>();
                    repos.AddRange(service.Get("rebeccapowell").Result);
                    repos.AddRange(service.Get("stevejgordon").Result);
                    repos.AddRange(service.Get("christiannagel").Result);

                    //foreach (var repo in repos)
                    //{
                    //    Console.WriteLine($"{repo.Name}");
                    //}
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            

            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}
