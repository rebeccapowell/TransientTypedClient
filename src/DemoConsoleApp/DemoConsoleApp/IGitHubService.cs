using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoConsoleApp
{
    public interface IGitHubService
    {
        Task<List<Repository>> Get(string user);
    }
}
