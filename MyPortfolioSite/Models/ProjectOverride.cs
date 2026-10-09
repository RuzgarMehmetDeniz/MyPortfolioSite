using System.ComponentModel.DataAnnotations;

namespace MyPortfolioSite.Models
{
    public class ProjectOverride
    {
        public int Id { get; set; }
        [Required, StringLength(200)] public string RepoName { get; set; } = "";
        [StringLength(120)] public string? Title { get; set; }
        [StringLength(1000)] public string? Description { get; set; }
        [StringLength(500)] public string? CoverUrl { get; set; }
        [StringLength(500)] public string? DemoUrl { get; set; }
        public int SortOrder { get; set; } = 100;
        public bool IsHidden { get; set; }
        public bool IsFeatured { get; set; }
        [StringLength(4000)] public string? ExtraImages { get; set; } // her satıra bir URL
    }
}