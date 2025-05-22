using System;
using System.Collections.Generic;
using WebScraper.Services;
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

        public int CommitsLast7Days { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? LastContributionDate { get; set; }
        public RepositoryStatus Status { get; set; } = RepositoryStatus.Unknown;
        // TODO: LastMaintainerResponse ? ? ? 
    }
}