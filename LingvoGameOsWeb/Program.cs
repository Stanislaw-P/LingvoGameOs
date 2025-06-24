using AspNetCore.Unobtrusive.Ajax;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Настройка строки подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// указываем тип пользователя и роли
builder.Services.AddIdentity<User, IdentityRole>()
                // устанавливаем тип хранилища - наш контекст
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

// подключение контекста бд
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));



// настройка cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie = new CookieBuilder
    {
        IsEssential = true
    };
});

builder.Services.AddTransient<IGamesRepository, GamesDbRepository>();
builder.Services.AddTransient<ILanguageLevelsRepository, LanguageLevelsDbRepository>();
builder.Services.AddTransient<IPlatformsRepository, PlatformsDbRepository>();
builder.Services.AddTransient<ISkillsLearningRepository, SkillsLearningDbRepository>();

// Добавление ненавязчивого Ajax
builder.Services.AddUnobtrusiveAjax();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// подключение аутентификации
app.UseAuthentication();

// подключение авторизации
app.UseAuthorization();

// Подключение ненавязчивого Ajax
app.UseUnobtrusiveAjax();

// инициализация администратора
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContextOptions = services.GetRequiredService<DbContextOptions<DatabaseContext>>();
    await IdentityInitializer.InitializeAsync(userManager, rolesManager, dbContextOptions);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
