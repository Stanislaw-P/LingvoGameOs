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

Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// ��������� ������ ����������� �� appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� ��� ������������ � ����
builder
    .Services.AddIdentity<User, IdentityRole>()
    // ������������� ��� ��������� - ��� ��������
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

// ����������� ��������� ��
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));

// ��������� cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie = new CookieBuilder { IsEssential = true };
});

builder.Services.AddTransient<IGamesRepository, GamesDbRepository>();
builder.Services.AddTransient<ILanguageLevelsRepository, LanguageLevelsDbRepository>();
builder.Services.AddTransient<IPlatformsRepository, PlatformsDbRepository>();
builder.Services.AddTransient<ISkillsLearningRepository, SkillsLearningDbRepository>();
builder.Services.AddTransient<IPendingGamesRepository, PendingGamesDbRepository>();
builder.Services.AddTransient<IReviewsRepository, ReviewsDbRepository>();
builder.Services.AddTransient<IFavoriteGamesRepository, FavoriteGamesDbRepository>();

// ���������� ������������� Ajax
builder.Services.AddUnobtrusiveAjax();

// �����������
builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
);

// ����������� �����������
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

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

// ����������� ��������������
app.UseAuthentication();

// ����������� �����������
app.UseAuthorization();

// ����������� ������������� Ajax
app.UseUnobtrusiveAjax();

// ������������
//app.UseSerilogRequestLogging();

// ������������� ��������������
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContextOptions = services.GetRequiredService<DbContextOptions<DatabaseContext>>();
    await IdentityInitializer.InitializeAsync(userManager, rolesManager, dbContextOptions);
}

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
