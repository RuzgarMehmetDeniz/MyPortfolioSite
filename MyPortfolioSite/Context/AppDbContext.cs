using Microsoft.EntityFrameworkCore;
using MyPortfolioSite.Models;

namespace MyPortfolioSite.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<ContactMessage> Messages => Set<ContactMessage>();

        public DbSet<ProjectOverride> Overrides => Set<ProjectOverride>();

        protected override void OnModelCreating(ModelBuilder b)
            => b.Entity<ProjectOverride>().HasIndex(o => o.RepoName).IsUnique();
    }
}