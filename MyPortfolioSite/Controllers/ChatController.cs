using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyPortfolioSite.Models;
using MyPortfolioSite.Services;

namespace MyPortfolioSite.Controllers
{
    public class ChatController : Controller
    {
        private const int DailyLimit = 300;
        private static readonly object _lock = new();
        private static DateTime _day = DateTime.UtcNow.Date;
        private static int _count;

        private readonly ChatService _chat;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ChatService chat, ILogger<ChatController> logger)
        { _chat = chat; _logger = logger; }

        [HttpPost("/api/chat"), ValidateAntiForgeryToken, EnableRateLimiting("chat")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest req, CancellationToken ct)
        {
            var msgs = (req?.Messages ?? new()).TakeLast(10).ToList();
            if (msgs.Count == 0 || msgs.Any(m => m.Role != "user" && m.Role != "assistant"
                    || string.IsNullOrWhiteSpace(m.Content) || m.Content.Length > 500))
                return BadRequest(new { error = "Geçersiz mesaj." });

            while (msgs.Count > 0 && msgs[0].Role != "user") msgs.RemoveAt(0);
            if (msgs.Count == 0 || msgs[^1].Role != "user")
                return BadRequest(new { error = "Geçersiz mesaj." });
            for (int i = 1; i < msgs.Count; i++)
                if (msgs[i].Role == msgs[i - 1].Role)
                    return BadRequest(new { error = "Geçersiz mesaj." });

            lock (_lock)
            {
                if (_day != DateTime.UtcNow.Date) { _day = DateTime.UtcNow.Date; _count = 0; }
                if (_count >= DailyLimit)
                    return StatusCode(429, new { error = "Bugünlük mesaj sınırı doldu. Lütfen iletişim formunu kullan." });
                _count++;
            }

            try
            {
                var reply = await _chat.AskAsync(msgs, ct);
                return Json(new { reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sohbet hatası");

                var detail = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                    ? ex.Message
                    : "Şu an cevap veremiyorum. Lütfen daha sonra tekrar dene.";

                return StatusCode(500, new { error = detail });
            }
        }
    }
}