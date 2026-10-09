using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MyPortfolioSite.Context;
using MyPortfolioSite.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<GitHubService>(c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd("portfolio-site");
    c.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

    var token = builder.Configuration["GitHub:Token"];
    if (!string.IsNullOrWhiteSpace(token))
        c.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
});

builder.Services.AddHttpClient<ChatService>(c => c.Timeout = TimeSpan.FromSeconds(30));

builder.Services.AddScoped<PortfolioService>();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=App_Data/portfolio.db";
var dbPath = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString).DataSource;
var dbDir = Path.GetDirectoryName(Path.GetFullPath(dbPath));
if (!string.IsNullOrEmpty(dbDir)) Directory.CreateDirectory(dbDir);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/yonetim/giris";
        o.AccessDeniedPath = "/yonetim/giris";
        o.Cookie.HttpOnly = true;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;

    o.AddPolicy("contact", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 3, Window = TimeSpan.FromMinutes(1) }));

    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(5) }));

    o.AddPolicy("chat", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 8, Window = TimeSpan.FromMinutes(1) }));
});

var app = builder.Build();

// Veritabaný yoksa oluþturur, bekleyen migration varsa uygular
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();        
app.UseAuthentication();    
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();