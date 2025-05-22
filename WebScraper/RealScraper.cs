using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebScraper.Models;

namespace WebScraper.Services
{
    public class HtmlAgilityPackScraper : IGitHubScraper
    {
        private readonly HttpClient _http;

        public HtmlAgilityPackScraper(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<RepositoryInfo> FetchRepositoryAsync(string url)
        {
            var html = await _http.GetStringAsync(url);
            var doc  = new HtmlDocument();
            doc.LoadHtml(html);

            var repo = new RepositoryInfo
            {
                Owner       = doc.DocumentNode.SelectSingleNode("//span[@class='author flex-self-stretch']/a")?.InnerText.Trim() ?? "",
                Name        = doc.DocumentNode.SelectSingleNode("//strong[@itemprop='name']/a")?.InnerText.Trim() ?? "",
                Description = doc.DocumentNode.SelectSingleNode("//p[@class='f4 my-3']")?.InnerText.Trim() ?? "No description",
                Stars       = doc.DocumentNode.SelectSingleNode("//*[@id='repo-stars-counter-star']")?.InnerText.Trim() ?? "No Stars",
                Forks       = doc.DocumentNode.SelectSingleNode("//*[@id='repo-network-counter']")?.InnerText.Trim() ?? "No Forks",
                OpenIssues  = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/issues') and contains(@class, 'selected')]//span[contains(@class, 'Counter')]")
                ?.InnerText.Trim() ?? "No Open Issues",
                TopContributors = ParseContributors(doc),
                Languages       = ParseLanguages(doc)
            };

            // TODO: repo.CommitsLast7Days
            // TODO: classify repo.Status

            return repo;
        }

        private int ParseInt(HtmlDocument doc, string xpath)
        {
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            return int.TryParse(node?.InnerText.Trim().Replace(",", ""), out var n) ? n : 0;
        }

         private List<Contributor> ParseContributors(HtmlDocument doc)
        {
            var xpath =
              "//div[h2[contains(., 'Contributors')]]" +
              "//a[contains(@href, '/') and (@data-hovercard-type='user' or @itemprop='author')]";

            return doc.DocumentNode.SelectNodes(xpath)
                   ?.Take(5)
                   .Select(a => new Contributor {
                        Username = a.GetAttributeValue("href", "")
                                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                                    .Last(),
                        
                        
                     })
                   .ToList()
               ?? new List<Contributor>();
        }
        private List<LanguageInfo> ParseLanguages(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//h2[text() = 'Languages']/following-sibling::ul[1]//li//a");
            if (nodes == null) return new List<LanguageInfo>();

            return nodes.Select(a => new LanguageInfo {
                Name = a.SelectSingleNode(".//span[1]")?.InnerText.Trim() ?? "[unknown]",
                        Percentage = double.TryParse(
                            a.SelectSingleNode(".//span[2]")?.InnerText.Trim().TrimEnd('%'),
                            out var pct) ? pct : 0
                    }).ToList();
        }
    }
}
