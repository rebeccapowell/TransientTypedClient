using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DemoConsoleApp.Shared;

namespace DemoComsoleAppAutofac
{
    public interface IMockController
    {
        Task<List<Repository>> Get(string name);
    }

    public class MockConstroller : IMockController
    {
        private readonly IGitHubService _gitHubService;

        public MockConstroller(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }
        public async Task<List<Repository>> Get(string name)
        {
            return await _gitHubService.Get(name);
        }
    }
}
