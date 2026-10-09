using System.Text.Json.Serialization;

namespace MyPortfolioSite.Models
{
    public class Project
    {
        // ---- GitHub'dan gelen alanlar ----
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("homepage")]
        public string? Homepage { get; set; }

        [JsonPropertyName("fork")]
        public bool Fork { get; set; }

        [JsonPropertyName("topics")]
        public List<string> Topics { get; set; } = new();

        [JsonPropertyName("default_branch")]
        public string DefaultBranch { get; set; } = "main";

        [JsonIgnore]
        public string? Cover { get; set; }

        // ---- Admin panelinden gelen düzenlemeler ----
        [JsonIgnore] public string? CustomTitle { get; set; }
        [JsonIgnore] public string? CustomDescription { get; set; }
        [JsonIgnore] public string? CustomCover { get; set; }
        [JsonIgnore] public string? CustomDemoUrl { get; set; }
        [JsonIgnore] public int SortOrder { get; set; } = 100;
        [JsonIgnore] public bool IsHidden { get; set; }
        [JsonIgnore] public bool IsFeatured { get; set; }
        [JsonIgnore] public List<string> ExtraImages { get; set; } = new();

        // ---- Sitede gösterilecek değerler (düzenleme varsa o, yoksa GitHub) ----
        [JsonIgnore] public string Title => CustomTitle ?? Name;
        [JsonIgnore] public string? ShownDescription => CustomDescription ?? Description;
        [JsonIgnore] public string? ShownCover => CustomCover ?? Cover;
        [JsonIgnore] public string? ShownDemo => CustomDemoUrl ?? Homepage;
    }
}