using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MyPortfolioSite.Controllers
{
    [Route("yonetim")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _cfg;
        public AccountController(IConfiguration cfg) => _cfg = cfg;

        [HttpGet("giris")]
        public IActionResult Login(string? returnUrl) { ViewBag.ReturnUrl = returnUrl; return View(); }

        [HttpPost("giris"), ValidateAntiForgeryToken, EnableRateLimiting("login")]
        public async Task<IActionResult> Login(string? username, string? password, string? returnUrl)
        {
            var u = _cfg["Admin:Username"]; var p = _cfg["Admin:Password"];
            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
            {
                ViewBag.Error = "Yönetici bilgileri ayarlanmamış.";
                return View();
            }

            bool ok = Eq(username ?? "", u) & Eq(password ?? "", p);
            if (!ok) { ViewBag.Error = "Kullanıcı adı ya da şifre hatalı."; return View(); }

            var id = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, u) },
                CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(new ClaimsPrincipal(id));

            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/yonetim/projeler");
        }

        [HttpPost("cikis"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private static bool Eq(string a, string b) => CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(a)), SHA256.HashData(Encoding.UTF8.GetBytes(b)));
    }
}