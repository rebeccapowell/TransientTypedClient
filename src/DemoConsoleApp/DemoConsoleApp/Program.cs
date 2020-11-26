using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

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

            var p = services.BuildServiceProvider();

            var gitHubService = p.GetService<IGitHubService>();
            var repos = new List<Repository>();
            repos.AddRange(gitHubService.Get("rebeccapowell").Result);
            repos.AddRange(gitHubService.Get("stevejgordon").Result);
            repos.AddRange(gitHubService.Get("christiannagel").Result);

            //foreach (var repo in repos)
            //{
            //    Console.WriteLine($"{repo.Name}");
            //}

            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}
