using DotNetEnv;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Load environment variables from .env file
Env.Load("../LingvoGameOsWeb/.env");
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get database connection string from configuration
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var dbPass = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_YANDEX")
    : Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_TimeWeb");

var connectionString = $"Host=localhost;Port=5432;Database={dbName};Username={dbUser};Password={dbPass}";

// Configure ASP.NET Core Identity with custom User and IdentityRole
builder
    .Services.AddIdentity<User, IdentityRole>()
    // Add Entity Framework store for Identity using our DatabaseContext
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

// Register DatabaseContext with SQLite provider
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));


builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
