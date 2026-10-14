using AspNet.Security.OAuth.VkId;
using AspNetCore.Unobtrusive.Ajax;
using DotNetEnv;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Security.Claims;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;
using LingvoGameOs.Shared.Services;
using LingvoGameOs.Db.Services;
using LingvoGameOs.Localization;
using LingvoGameOs.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
// overwrite: false — переменные окружения (docker compose) приоритетнее файла
Env.Load(options: new LoadOptions(clobberExistingVars: false));
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = SupportedLanguages.All
        .Select(culture => new System.Globalization.CultureInfo(culture))
        .ToArray();

    options.DefaultRequestCulture = new RequestCulture(SupportedLanguages.Default);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = [new CookieRequestCultureProvider()];
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SecondLocalizer>();

var gameProxyRoutes = GameProxyCatalog.All.Select(game =>
    new RouteConfig
    {
        RouteId = $"game-{game.Slug}",
        ClusterId = $"game-{game.Slug}",
        Match = new RouteMatch { Path = $"/games/{game.Slug}/{{**catch-all}}" },
        Transforms =
        [
            new Dictionary<string, string>
            {
                ["PathRemovePrefix"] = $"/games/{game.Slug}"
            },
            new Dictionary<string, string>
            {
                // YARP normally derives this header from Request.PathBase. The
                // public game prefix is removed by a route transform instead,
                // so keep the default transform from overwriting our value.
                ["X-Forwarded"] = "Set",
                ["Prefix"] = "Off"
            },
            new Dictionary<string, string>
            {
                ["RequestHeader"] = "X-Forwarded-Prefix",
                ["Set"] = $"/games/{game.Slug}"
            }
        ]
    }).ToArray();

var gameProxyClusters = GameProxyCatalog.All.Select(game =>
    new ClusterConfig
    {
        ClusterId = $"game-{game.Slug}",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["game"] = new() { Address = game.Destination }
        }
    }).ToArray();

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(gameProxyRoutes, gameProxyClusters);

// Get database connection string from configuration
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var dbPass = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ||
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test"
    ? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_YANDEX")
    : Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_TimeWeb");

var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var connectionString = $"Host={dbHost};Port=5432;Database={dbName};Username={dbUser};Password={dbPass}";


// Configure ASP.NET Core Identity with custom User and IdentityRole
builder
    .Services.AddIdentity<User, IdentityRole>()
    // Add Entity Framework store for Identity using our DatabaseContext
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<RussianIdentityErrorDescriber>();

// Register DatabaseContext with SQLite provider
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));

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
builder.Services.AddTransient<IGameSessionRepository, GameSessionRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// Встраивание зависимости для работы с почтой
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<EmailService>();

builder.Services.AddScoped<GameSessionService>();

// S3 or local storage Service for file
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    builder.Services.AddScoped<IFileStorage, LocalFileProvider>();
}
else
{
    builder.Services.AddScoped<IFileStorage, S3Service>();
}

builder.Services.AddScoped<GameFileProcessor>();

builder.Services.AddScoped<RatingService>();

// Add support for unobtrusive AJAX functionality
builder.Services.AddUnobtrusiveAjax();

// Configure Serilog for application logging
builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
);

// Add in-memory caching service
builder.Services.AddMemoryCache();
builder.Services.AddHostedService<ReviewCache>();

// ←←← ЭТО ОБЯЗАТЕЛЬНО ДО UseAuthentication()
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    // Важно для новых версий .NET 8.0.17+ и выше (security hardening)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();

    // Nginx работает локально → разрешаем loopback
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Loopback, 8));     // 127.0.0.0/8
    options.KnownNetworks.Add(new IPNetwork(IPAddress.IPv6Loopback, 128));
});

var authenticationBuilder = builder.Services.AddAuthentication();
var vkIdClientId = Environment.GetEnvironmentVariable("VKID_CLIENT_ID");
var vkIdClientSecret = Environment.GetEnvironmentVariable("VKID_CLIENT_SECRET");
if (!string.IsNullOrWhiteSpace(vkIdClientId) && !string.IsNullOrWhiteSpace(vkIdClientSecret))
{
    authenticationBuilder.AddVkId(options =>
    {
        options.ClientId = vkIdClientId;
        options.ClientSecret = vkIdClientSecret;
        options.CallbackPath = "/signin-vkid"; // URL для callback
        options.Scope.Add("email"); // Запрашиваем email
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        options.ClaimActions.MapJsonKey("photo", "avatar");
    });
}

var app = builder.Build();

// test
app.UseForwardedHeaders();
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
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

// Миграции применяются один раз при старте (не в каждом scoped DbContext)
using (var serviceScope = app.Services.CreateScope())
{
    var db = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

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
foreach (var game in GameProxyCatalog.All)
{
    var publicPath = $"/games/{game.Slug}";
    app.MapGet(publicPath, context =>
    {
        context.Response.Redirect($"{publicPath}/{context.Request.QueryString}");
        return Task.CompletedTask;
    });
}

app.MapReverseProxy();

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseRouting();

// Добавьте ЭТУ строку - она будет перенаправлять все ошибки 404 на ваш контроллер
app.UseStatusCodePagesWithReExecute("/error/{0}");


app.Run();
