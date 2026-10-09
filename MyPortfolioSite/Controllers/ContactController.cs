using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyPortfolioSite.Context;
using MyPortfolioSite.Models;

namespace MyPortfolioSite.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _db;
        public ContactController(AppDbContext db) => _db = db;

        [HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("contact")]
        public async Task<IActionResult> Send(ContactMessage model, string? website)
        {
            if (!string.IsNullOrEmpty(website))
            {
                TempData["ContactOk"] = "Mesajın gönderildi, teşekkürler!";
                return Redirect("/#iletisim");
            }

            if (!ModelState.IsValid)
            {
                TempData["ContactError"] = ModelState.Values
                    .SelectMany(v => v.Errors).First().ErrorMessage;
                return Redirect("/#iletisim");
            }

            model.Id = 0;
            model.CreatedAt = DateTime.UtcNow;
            model.IsRead = false;
            _db.Messages.Add(model);
            await _db.SaveChangesAsync();

            TempData["ContactOk"] = "Mesajın gönderildi, teşekkürler!";
            return Redirect("/#iletisim");
        }
    }
}