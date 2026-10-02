using DotNetEnv;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Load environment variables from .env file
// overwrite: false — переменные окружения (docker compose) приоритетнее файла
Env.Load("../LingvoGameOsWeb/.env", new LoadOptions(clobberExistingVars: false));
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get database connection string from configuration
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var dbPass = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_YANDEX")
    : Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_TimeWeb");

var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var connectionString = $"Host={dbHost};Port=5432;Database={dbName};Username={dbUser};Password={dbPass}";

// Configure ASP.NET Core Identity with custom User and IdentityRole
builder
    .Services.AddIdentity<User, IdentityRole>()
    // Add Entity Framework store for Identity using our DatabaseContext
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

// Register DatabaseContext with SQLite provider
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Rate limiting: логин — 5 запросов/мин на IP, score — 20 запросов/мин на IP
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

    options.AddPolicy("score", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Миграции применяются один раз при старте (не в каждом scoped DbContext)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
