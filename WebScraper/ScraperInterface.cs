using System.Threading.Tasks;
using WebScraper.Models;

namespace WebScraper.Services
{
    public interface IGitHubScraper
    {
        Task<RepositoryInfo> FetchRepositoryAsync(string url);
    }
}