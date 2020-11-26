using System;
using Microsoft.Extensions.DependencyInjection;

namespace DemoConsoleApp
{
    public interface IGitHubServiceFactory
    {
        IGitHubService GetService();
    }

    public class GitHubServiceFactory : IGitHubServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GitHubServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGitHubService GetService()
        {
            return _serviceProvider.GetRequiredService<IGitHubService>();
        }
    }
}
