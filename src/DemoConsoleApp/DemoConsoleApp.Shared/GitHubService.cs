using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoConsoleApp.Shared
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Repository>> Get(string user)
        {
            var instance = _httpClient
                .DefaultRequestHeaders
                .FirstOrDefault(x =>
                    x.Key.Equals("instance", StringComparison.InvariantCultureIgnoreCase))
                .Value.FirstOrDefault();
            Console.WriteLine($"Instance: {instance}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{user}/repos");
            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsAsync<List<Repository>>();
        }
    }

    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content) =>
            await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync());
    }
}
