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
            //Getting input URL from user
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
                 var _LastSevenDays = new LastSevenDays(); 
                repo.CommitsLast7Days = await _LastSevenDays.GetCommitCountLast7DaysAsync(repo.Owner, repo.Name);
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
            //Displaying top 5 contributors in a for loop to output them in a counter-like fashion
            //It is made this way only for output style reasons
            for (int i = 0; i < r.TopContributors.Count; i++)
            {
                var contributor = r.TopContributors[i];
                Console.WriteLine($"{i + 1}. {contributor.Username}");
            }
            Console.WriteLine("\nLanguages:");
            
            r.Languages.ForEach(l =>
                Console.WriteLine($" - {l.Name} {l.Percentage}%"));

            Console.WriteLine($"\nCommits in the last 7 days: {r.CommitsLast7Days}");
            



        }
    }
}
