using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebScraper.Models;

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

        public Activity(string token, string userAgent = "WebScraperApp")
    {
        _client = new GitHubClient(new ProductHeaderValue(userAgent));
        if (!string.IsNullOrWhiteSpace(token))
            _client.Credentials = new Credentials(token);
        else
            Console.WriteLine("No token provided; continuing unauthenticated (60 req/hr).");
    }
        
        public async Task LoadAsync(RepositoryInfo repo)
        {
            var ghRepo = await _client.Repository.Get(repo.Owner, repo.Name);
            repo.IsArchived = ghRepo.Archived;
            var sinceOneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);
            var commits = await _client.Repository.Commit.GetAll(
                repo.Owner,
                repo.Name,
                new CommitRequest { Since = sinceOneYearAgo }
            );
            var mostRecent = commits
                .OrderByDescending(c => c.Commit.Author.Date)
                .FirstOrDefault();

            repo.LastContributionDate = mostRecent?
                .Commit.Author.Date
                .UtcDateTime;
        }
 public static void EvaluateStatus(RepositoryInfo repo)
        {
            var now = DateTime.UtcNow;
            if (repo.IsArchived ||
                (repo.LastContributionDate.HasValue &&
                 repo.LastContributionDate.Value < now.AddYears(-1)))
            {
                repo.Status = RepositoryStatus.Dead;
                return;
            }
            if (repo.LastContributionDate.HasValue &&
                repo.LastContributionDate.Value >= now.AddMonths(-6) &&
                !repo.IsArchived)
            {
                repo.Status = RepositoryStatus.Live;
                return;
            }
            repo.Status = RepositoryStatus.Stagnant;
        }
    }
}