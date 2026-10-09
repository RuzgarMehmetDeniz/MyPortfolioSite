using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioSite.Context;
using MyPortfolioSite.Models;
using MyPortfolioSite.Services;

namespace MyPortfolioSite.Controllers
{
    [Authorize, Route("yonetim/projeler")]
    public class AdminProjectController : Controller
    {
        private readonly PortfolioService _portfolio;
        private readonly GitHubService _gh;
        private readonly AppDbContext _db;

        public AdminProjectController(PortfolioService portfolio, GitHubService gh, AppDbContext db)
        { _portfolio = portfolio; _gh = gh; _db = db; }

        [HttpGet("")]
        public async Task<IActionResult> Index() => View(await _portfolio.GetAllAsync());

        [HttpGet("duzenle/{name}")]
        public async Task<IActionResult> Edit(string name)
        {
            var project = await _portfolio.GetAsync(name, includeHidden: true);
            if (project is null) return NotFound();
            ViewBag.Project = project;

            var o = await _db.Overrides.AsNoTracking().FirstOrDefaultAsync(x => x.RepoName == name);

            var model = new ProjectOverride
            {
                RepoName = name,
                Title = project.Title,
                Description = project.ShownDescription,
                CoverUrl = project.ShownCover,
                DemoUrl = project.ShownDemo,
                ExtraImages = o?.ExtraImages,
                SortOrder = o?.SortOrder ?? 100,
                IsHidden = o?.IsHidden ?? false,
                IsFeatured = o?.IsFeatured ?? false
            };
            return View(model);
        }

        [HttpPost("duzenle/{name}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string name, ProjectOverride model)
        {
            var project = await _portfolio.GetAsync(name, includeHidden: true);
            if (project is null) return NotFound();

            ModelState.Remove(nameof(model.RepoName));
            model.RepoName = name;

            if (!TryUrl(model.CoverUrl, out var cover))
                ModelState.AddModelError(nameof(model.CoverUrl), $"Kapak adresi geçersiz: {model.CoverUrl}");

            if (!TryUrl(model.DemoUrl, out var demo))
                ModelState.AddModelError(nameof(model.DemoUrl), $"Demo adresi geçersiz: {model.DemoUrl}");

            var extra = new List<string>();
            foreach (var line in (model.ExtraImages ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (TryUrl(line, out var fixedLine) && fixedLine != null) extra.Add(fixedLine);
                else ModelState.AddModelError(nameof(model.ExtraImages), $"Ek görsel adresi geçersiz: {line}");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Project = project;
                return View(model);
            }

            var o = await _db.Overrides.FirstOrDefaultAsync(x => x.RepoName == name);
            if (o is null) { o = new ProjectOverride { RepoName = name }; _db.Overrides.Add(o); }

            o.Title = SameOrNull(model.Title, project.Name);
            o.Description = SameOrNull(model.Description, project.Description);
            o.CoverUrl = SameOrNull(cover, project.Cover);
            o.DemoUrl = SameOrNull(demo, project.Homepage);
            o.ExtraImages = extra.Count == 0 ? null : string.Join("\n", extra);
            o.SortOrder = model.SortOrder;
            o.IsHidden = model.IsHidden;
            o.IsFeatured = model.IsFeatured;

            await _db.SaveChangesAsync();
            TempData["Ok"] = "Kaydedildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("gizle/{name}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHidden(string name)
        {
            var o = await _db.Overrides.FirstOrDefaultAsync(x => x.RepoName == name);
            if (o is null) { o = new ProjectOverride { RepoName = name }; _db.Overrides.Add(o); }
            o.IsHidden = !o.IsHidden;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("sifirla/{name}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset(string name)
        {
            var o = await _db.Overrides.FirstOrDefaultAsync(x => x.RepoName == name);
            if (o != null) { _db.Overrides.Remove(o); await _db.SaveChangesAsync(); }
            TempData["Ok"] = "GitHub'daki değerlere dönüldü.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("yenile"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Refresh()
        {
            var names = (await _gh.GetProjectsAsync()).Select(p => p.Name).ToList();
            _gh.ClearCache(names);
            TempData["Ok"] = "GitHub verisi yenilendi.";
            return RedirectToAction(nameof(Index));
        }

        private static string? SameOrNull(string? value, string? github)
        {
            var v = value?.Trim();
            if (string.IsNullOrEmpty(v)) return null;
            return v == github?.Trim() ? null : v;
        }

        private static bool TryUrl(string? input, out string? result)
        {
            result = null;
            var u = input?.Trim();
            if (string.IsNullOrEmpty(u)) return true;

            if (!u.Contains("://")) u = "https://" + u;

            if (Uri.TryCreate(u, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                !string.IsNullOrEmpty(uri.Host))
            {
                result = u;
                return true;
            }
            return false;
        }
    }
}