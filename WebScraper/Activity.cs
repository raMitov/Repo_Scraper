using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace WebScraper.Services
{
    public enum RepositoryStatus
    {
        Live,
        Stagnant,
        Dead,
        Unknown
}
    public class Activity
    {
        private readonly GitHubClient _client;

        public Activity()
        {
            _client = new GitHubClient(new ProductHeaderValue("WebScraperApp"));

        }

        public async Task<bool> IsArchivedAsync(string owner, string repoName)
        {
            var repo = await _client.Repository.Get(owner, repoName);
            return repo.Archived;
        }
    }
}