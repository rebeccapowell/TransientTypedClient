using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DemoConsoleApp.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace DemoComsoleAppAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IGitHubService, GitHubService>("MyHttpClient")
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "My User Agent Example");
                    client.DefaultRequestHeaders.Add("Instance", Guid.NewGuid().ToString());
                    client.BaseAddress = new Uri("https://api.github.com/");
                });

            var containerBuilder = new ContainerBuilder();

            // Once you've registered everything in the ServiceCollection, call
            // Populate to bring those registrations into Autofac. This is
            // just like a foreach over the list of things in the collection
            // to add them to Autofac.
            containerBuilder.Populate(services);

            // Make your Autofac registrations. Order is important!
            // If you make them BEFORE you call Populate, then the
            // registrations in the ServiceCollection will override Autofac
            // registrations; if you make them AFTER Populate, the Autofac
            // registrations will override. You can make registrations
            // before or after Populate, however you choose.
            containerBuilder.Register(c => c.Resolve<IHttpClientFactory>().CreateClient("MyHttpClient"))
                .As<HttpClient>();

            // Creating a new AutofacServiceProvider makes the container
            // available to your app using the Microsoft IServiceProvider
            // interface so you can use those abstractions rather than
            // binding directly to Autofac.
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            var service = serviceProvider.GetService<IGitHubService>();

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
        }
    }
}
