using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebScraper.Services;

namespace WebScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter GitHub repo URL: ");
            var url = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("Invalid URL!");
                return;
            }

            using var http = new HttpClient();
            IGitHubScraper scraper = new HtmlAgilityPackScraper(http);

            try
            {
                var repo = await scraper.FetchRepositoryAsync(url);
                Display(repo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching repository: {ex.Message}");
            }
        }

        private static void Display(Models.RepositoryInfo r)
        {
            Console.WriteLine($"\nRepository: {r.Owner}/{r.Name}");
            Console.WriteLine($"Description: {r.Description}");
            Console.WriteLine($"Stars: {r.Stars}   Forks: {r.Forks}   Open issues: {r.OpenIssues}");
            
            Console.WriteLine("\nTop Contributors:");
            for (int i = 0; i < r.TopContributors.Count; i++)
            {
             var contributor = r.TopContributors[i];
             Console.WriteLine($"{i + 1}. {contributor.Username}");
            }
            Console.WriteLine("\nLanguages:");
            r.Languages.ForEach(l => 
                Console.WriteLine($" - {l.Name} {l.Percentage}%"));
            
            // TODO: display CommitsLast7Days, PR stats, Issue stats, Status
        }
    }
}
