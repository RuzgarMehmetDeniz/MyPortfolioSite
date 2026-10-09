using System.ComponentModel.DataAnnotations;

namespace MyPortfolioSite.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim gerekli"), StringLength(80)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "E-posta gerekli"), EmailAddress(ErrorMessage = "Geçerli bir e-posta yaz"), StringLength(120)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Mesaj gerekli"), StringLength(2000, MinimumLength = 10, ErrorMessage = "Mesaj en az 10 karakter olmalı")]
        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}