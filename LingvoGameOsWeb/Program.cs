using System;
using AspNetCore.Unobtrusive.Ajax;
using DotNetEnv;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get database connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure ASP.NET Core Identity with custom User and IdentityRole
builder
    .Services.AddIdentity<User, IdentityRole>()
    // Add Entity Framework store for Identity using our DatabaseContext
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

// Register DatabaseContext with SQLite provider
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));

// Configure application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie = new CookieBuilder { IsEssential = true };
});


// Register repository interfaces and implementations for dependency injection
builder.Services.AddTransient<IGamesRepository, GamesDbRepository>();
builder.Services.AddTransient<ILanguageLevelsRepository, LanguageLevelsDbRepository>();
builder.Services.AddTransient<IPlatformsRepository, PlatformsDbRepository>();
builder.Services.AddTransient<ISkillsLearningRepository, SkillsLearningDbRepository>();
builder.Services.AddTransient<IPendingGamesRepository, PendingGamesDbRepository>();
builder.Services.AddTransient<IReviewsRepository, ReviewsDbRepository>();
builder.Services.AddTransient<IFavoriteGamesRepository, FavoriteGamesDbRepository>();

// Add support for unobtrusive AJAX functionality
builder.Services.AddUnobtrusiveAjax();

// Configure Serilog for application logging
builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
);

// Add in-memory caching service
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Serve static files with caching headers
app.UseStaticFiles(
    new StaticFileOptions()
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
        },
    }
);

app.UseRouting();

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Enable unobtrusive AJAX support
app.UseUnobtrusiveAjax();

// Uncomment to enable Serilog request logging
//app.UseSerilogRequestLogging();

// Initialize identity system with default roles and users
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContextOptions = services.GetRequiredService<DbContextOptions<DatabaseContext>>();
    var conf = services.GetRequiredService<IConfiguration>();
    await IdentityInitializer.InitializeAsync(userManager, rolesManager, dbContextOptions, conf);
}

// Configure area routing
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
