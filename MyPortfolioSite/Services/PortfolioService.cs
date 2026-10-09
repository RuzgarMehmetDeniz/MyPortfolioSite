using Microsoft.EntityFrameworkCore;
using MyPortfolioSite.Context;
using MyPortfolioSite.Models;

namespace MyPortfolioSite.Services
{
    public class PortfolioService
    {
        private readonly GitHubService _gh;
        private readonly AppDbContext _db;
        public PortfolioService(GitHubService gh, AppDbContext db) { _gh = gh; _db = db; }

        public async Task<List<Project>> GetAllAsync()
        {
            var list = await _gh.GetProjectsAsync();
            var ov = await _db.Overrides.AsNoTracking().ToDictionaryAsync(o => o.RepoName);
            foreach (var p in list) Apply(p, ov.GetValueOrDefault(p.Name));
            return list;
        }

        public async Task<List<Project>> GetVisibleAsync() =>
            (await GetAllAsync()).Where(p => !p.IsHidden)
                .OrderByDescending(p => p.IsFeatured).ThenBy(p => p.SortOrder).ToList();

        public async Task<Project?> GetAsync(string name, bool includeHidden = false) =>
            (await GetAllAsync()).FirstOrDefault(p => p.Name == name && (includeHidden || !p.IsHidden));

        private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static void Apply(Project p, ProjectOverride? o)
        {
            p.CustomTitle = Clean(o?.Title);
            p.CustomDescription = Clean(o?.Description);
            p.CustomCover = Clean(o?.CoverUrl);
            p.CustomDemoUrl = Clean(o?.DemoUrl);
            p.SortOrder = o?.SortOrder ?? 100;
            p.IsHidden = o?.IsHidden ?? false;
            p.IsFeatured = o?.IsFeatured ?? false;
            p.ExtraImages = (o?.ExtraImages ?? "")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
    }
}