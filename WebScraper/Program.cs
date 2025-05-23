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
            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
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
            //Big try-catch, if something doesnt work, nothing workds :D
            try
            {
                Console.WriteLine("1) Scraping HTML…");
                var repo = await scraper.FetchRepositoryAsync(url);
                var activity = new Activity(token);
                 Console.WriteLine("2) Loading archived + last-contribution via API…");
                await activity.LoadAsync(repo);
                Console.WriteLine("3) Counting commits in last 7 days…");
                var _LastSevenDays = new LastSevenDays(token);
                repo.CommitsLast7Days = await _LastSevenDays.GetCommitCountLast7DaysAsync(repo.Owner, repo.Name);
                Console.WriteLine("4) Evaluating status…");
                Activity.EvaluateStatus(repo);
                Console.WriteLine("5) Displaying results…");
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


            Console.WriteLine($"\nArchived: {r.IsArchived}");
            Console.WriteLine($"Last contribution date: {(r.LastContributionDate?.ToString("yyyy-MM-dd") ?? "None")}");
            Console.WriteLine($"Status: {r.Status}");
            



        }
    }
}
