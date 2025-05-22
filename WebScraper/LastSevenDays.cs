using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebScraper.Services
{
    //A class made to process the commits in the last 7 days in the repository.
    public class LastSevenDays
    {
        private readonly HttpClient _httpClient;
        public LastSevenDays()
        {
            //Making a client and issuing a request to github API
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");
        }

        public async Task<int> GetCommitCountLast7DaysAsync(string owner, string repo)
        {
            //Date since we want to get the contributions, we subtract 7 to get the last 7 days and convert it to ISO standart
            var since = DateTime.UtcNow.AddDays(-7).ToString("o");
            var url = $"https://api.github.com/repos/{owner}/{repo}/commits?since={since}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            //Deserialise the Json to get just the count.
            var commits = JsonSerializer.Deserialize<List<object>>(json);
            return commits?.Count ?? 0;
        }
    }
}