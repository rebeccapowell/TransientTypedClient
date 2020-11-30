using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DemoConsoleApp.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace DemoComsoleAppAutofac
{
    class Program
    {
        static void Main(string[] args) => Run().GetAwaiter().GetResult();

        public static async Task Run()
        {
            await RunIncorrectBehavior();
            await RunCorrectBehavior();

            Console.ReadLine();
        }

        public static async Task RunIncorrectBehavior()
        {
            Console.WriteLine($"Doing it wrong");
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
            containerBuilder.RegisterType<MockConstroller>().As<IMockController>()
                .InstancePerLifetimeScope();

            // Creating a new AutofacServiceProvider makes the container
            // available to your app using the Microsoft IServiceProvider
            // interface so you can use those abstractions rather than
            // binding directly to Autofac.
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            var repos = new List<Repository>();
            var stopWatch = new Stopwatch();

            // mock request 1
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // mock request 2
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("stevejgordon"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // mock request 3
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("christiannagel"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            Console.WriteLine($"{repos.Count}");
        }

        public static async Task RunCorrectBehavior()
        {
            Console.WriteLine($"Doing it right");
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

            containerBuilder.Register(ctx =>
            {
                var factory = ctx.Resolve<IHttpClientFactory>();
                var client = factory.CreateClient("MyHttpClient");
                return client;
            })
                .Named<HttpClient>("MyHttpClient")
                .SingleInstance();
            containerBuilder.Register(ctx =>
                    new GitHubService(ctx.ResolveNamed<HttpClient>("MyHttpClient")))
                .As<IGitHubService>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<MockConstroller>().As<IMockController>()
                .InstancePerLifetimeScope();


            // Creating a new AutofacServiceProvider makes the container
            // available to your app using the Microsoft IServiceProvider
            // interface so you can use those abstractions rather than
            // binding directly to Autofac.
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            var repos = new List<Repository>();
            var stopWatch = new Stopwatch();

            // mock request 1
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // mock request 2
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("stevejgordon"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // mock request 3
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("christiannagel"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            // mock request 4
            stopWatch.Start();
            await using (var lifetime = serviceProvider.LifetimeScope.BeginLifetimeScope())
            {
                var service = lifetime.Resolve<IMockController>();
                repos.AddRange(await service.Get("rebeccapowell"));
                repos.AddRange(await service.Get("rebeccapowell"));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            }

            Console.WriteLine($"{repos.Count}");
        }
    }
}
