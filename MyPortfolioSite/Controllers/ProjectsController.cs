using Microsoft.AspNetCore.Mvc;
using MyPortfolioSite.Services;

namespace MyPortfolioSite.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly PortfolioService _portfolio;
        private readonly GitHubService _gitHub;

        public ProjectsController(PortfolioService portfolio, GitHubService gitHub)
        {
            _portfolio = portfolio;
            _gitHub = gitHub;
        }

        public async Task<IActionResult> Detail(string id)
        {
            var project = await _portfolio.GetAsync(id);
            if (project is null) return NotFound();

            var readme = await _gitHub.GetReadmeAsync(project);
            var parsed = _gitHub.ParseReadme(readme, project);

            ViewBag.Readme = parsed.Text;
            ViewBag.Images = project.ExtraImages.Concat(parsed.Images).Distinct().ToList();
            return View(project);
        }

        public async Task<IActionResult> Show(string id)
        {
            var project = await _portfolio.GetAsync(id);
            if (project is null) return NotFound();

            var readme = await _gitHub.GetReadmeAsync(project);
            var parsed = _gitHub.ParseReadme(readme, project);

            ViewBag.Images = project.ExtraImages.Concat(parsed.Images).Distinct().ToList();
            return View(project);
        }
    }
}