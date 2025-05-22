using System.Collections.Generic;
namespace WebScraper.Models
{
    public class RepositoryInfo
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Stars { get; set; }
        public string Forks { get; set; }
        public string OpenIssues { get; set; }
        public List<Contributor> TopContributors { get; set; } = new();
        public List<LanguageInfo> Languages { get; set; } = new();
        // TODO: CommitsLast7Days
    }
}