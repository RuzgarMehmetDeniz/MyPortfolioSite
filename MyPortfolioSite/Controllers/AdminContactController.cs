using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioSite.Context;

namespace MyPortfolioSite.Controllers
{
    [Authorize, Route("yonetim/mesajlar")]
    public class AdminContactController : Controller
    {
        private readonly AppDbContext _db;
        public AdminContactController(AppDbContext db) => _db = db;

        [HttpGet("")]
        public async Task<IActionResult> Index() =>
            View(await _db.Messages.AsNoTracking().OrderByDescending(m => m.CreatedAt).ToListAsync());

        [HttpPost("okundu/{id:int}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRead(int id)
        {
            var m = await _db.Messages.FindAsync(id);
            if (m != null) { m.IsRead = !m.IsRead; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("sil/{id:int}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.Messages.FindAsync(id);
            if (m != null) { _db.Messages.Remove(m); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}