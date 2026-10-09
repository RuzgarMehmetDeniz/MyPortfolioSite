using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyPortfolioSite.Models;
using MyPortfolioSite.Services;

namespace MyPortfolioSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PortfolioService _portfolio;

        public HomeController(ILogger<HomeController> logger, PortfolioService portfolio)
        {
            _logger = logger;
            _portfolio = portfolio;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 12;

            var projects = await _portfolio.GetVisibleAsync();

            var totalPages = (int)Math.Ceiling(projects.Count / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            page = Math.Clamp(page, 1, totalPages);

            var pageItems = projects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(pageItems);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}