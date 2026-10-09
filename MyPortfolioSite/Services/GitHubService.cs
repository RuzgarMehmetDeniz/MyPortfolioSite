using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolioSite.Models;

namespace MyPortfolioSite.Services
{
    public class GitHubService
    {
        private const string User = "RuzgarMehmetDeniz";
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(1);

        private static List<Project>? _lastGood;

        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;

        public GitHubService(HttpClient http, IMemoryCache cache)
        {
            _http = http;
            _cache = cache;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            var result = await _cache.GetOrCreateAsync("projects", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTime;

                try
                {
                    var list = new List<Project>();
                    for (var page = 1; ; page++)
                    {
                        var batch = await _http.GetFromJsonAsync<List<Project>>(
                            $"users/{User}/repos?per_page=100&page={page}&sort=updated")
                            ?? new List<Project>();

                        list.AddRange(batch);
                        if (batch.Count < 100) break;
                    }

                    var projects = list.Where(p => !p.Fork && p.Name != User).ToList();

                    await Task.WhenAll(projects.Select(async p =>
                    {
                        var md = await GetReadmeAsync(p);
                        p.Cover = ParseReadme(md, p).Images.FirstOrDefault();
                    }));

                    _lastGood = projects;
                    return projects;
                }
                catch
                {
                    if (_lastGood != null) return _lastGood;
                    throw;
                }
            });

            return result ?? new List<Project>();
        }

        public async Task<Project?> GetProjectAsync(string name)
        {
            var projects = await GetProjectsAsync();
            return projects.FirstOrDefault(p => p.Name == name);
        }

        public async Task<string> GetReadmeAsync(Project p)
        {
            return await _cache.GetOrCreateAsync("readme:" + p.Name, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTime;

                foreach (var file in new[] { "README.md", "readme.md", "Readme.md", "README.MD", "README.markdown" })
                {
                    try
                    {
                        var res = await _http.GetAsync(
                            $"https://raw.githubusercontent.com/{User}/{p.Name}/{p.DefaultBranch}/{file}");
                        if (res.IsSuccessStatusCode) return await res.Content.ReadAsStringAsync();
                    }
                    catch { }
                }
                return "";
            }) ?? "";
        }

        public (string Text, List<string> Images) ParseReadme(string md, Project p)
        {
            var images = new List<string>();

            string Resolve(string u)
            {
                if (u.StartsWith("http")) return u;
                u = u.TrimStart('/');
                while (u.StartsWith("./")) u = u[2..];
                return $"https://raw.githubusercontent.com/{User}/{p.Name}/{p.DefaultBranch}/{u}";
            }

            void Add(Match m)
            {
                var u = m.Groups[1].Value;
                if (!Regex.IsMatch(u, "shields\\.io|badge", RegexOptions.IgnoreCase))
                    images.Add(Resolve(u));
            }

            foreach (Match m in Regex.Matches(md, @"!\[[^\]]*\]\(([^)\s]+)[^)]*\)")) Add(m);
            foreach (Match m in Regex.Matches(md, "<img[^>]+src=\"([^\"]+)\"")) Add(m);

            var text = Regex.Replace(md, @"!\[[^\]]*\]\([^)]*\)", "");
            text = Regex.Replace(text, "<[^>]+>", "");
            text = Regex.Replace(text, @"^#{1,6}\s*", "", RegexOptions.Multiline);
            text = Regex.Replace(text, @"\[([^\]]+)\]\([^)]*\)", "$1");
            text = Regex.Replace(text, @"[*_`]{1,3}", "");
            text = Regex.Replace(text, @"\n{3,}", "\n\n").Trim();

            return (text, images.Distinct().ToList());
        }

        public void ClearCache(IEnumerable<string>? names = null)
        {
            _cache.Remove("projects");
            foreach (var n in names ?? Enumerable.Empty<string>()) _cache.Remove("readme:" + n);
        }
    }
}