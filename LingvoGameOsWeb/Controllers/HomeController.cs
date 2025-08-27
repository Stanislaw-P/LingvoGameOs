using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    readonly IGamesRepository gamesRepository;
    readonly DatabaseContext newDatabaseContext;
    readonly UserManager<User> userManager;
    public HomeController(IGamesRepository gamesRepository, DatabaseContext newDatabaseContext, UserManager<User> userManager)
    {
        this.gamesRepository = gamesRepository;
        this.newDatabaseContext = newDatabaseContext;
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var games = await gamesRepository.GetAllAsync();
        ViewBag.SkillsLearning = newDatabaseContext.SkillsLearning.Select(type => type.Name);
        return View(games);
    }
    [HttpPost]
    public async Task<IActionResult> Index(string review)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Index");

        var reviewData = new
        {
            text = review,
            author = $"{user.Name} {user.Surname}",
            date = DateTime.Now
        };

        // Простой путь - в корень проекта
        string filePath = "reviews.json";
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(reviewData, options);

        await System.IO.File.WriteAllTextAsync(filePath, json);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Search(string gameName)
    {
        var games = await gamesRepository.GetAllAsync();

        if (gameName != null)
        {
            games = games.Where(game => Regex.IsMatch(game.Title, gameName, RegexOptions.IgnoreCase)).ToList();
            ViewBag.GameName = gameName;
        }

        return PartialView("_GamesListPartial", games);
    }

    public async Task<IActionResult> FullGamesList()
    {
        var games = await gamesRepository.GetAllAsync();
        return PartialView("_GamesListPartial", games);
    }

    public async Task<IActionResult> News()
    {
        return View();
    }

    public async Task<IActionResult> Games()
    {
        var games = await gamesRepository.GetAllAsync();
        ViewBag.SkillsLearning = newDatabaseContext.SkillsLearning.Select(type => type.Name);
        return View(games);
    }

    public async Task<IActionResult> Categories()
    {
        var games = await gamesRepository.GetAllAsync();
        ViewBag.SkillsLearning = newDatabaseContext.SkillsLearning.Select(type => type.Name);
        return View(games);
    }

    public async Task<IActionResult> CategoryGames(string category)
    {
        var games = await gamesRepository.GetAllAsync();
        var filteredGames = games.Where(game => 
            game.SkillsLearning != null && 
            game.SkillsLearning.Any(skill => 
                skill.Name.Equals(category, StringComparison.OrdinalIgnoreCase)
            )
        ).ToList();
        
        ViewBag.Category = category;
        ViewBag.SkillsLearning = newDatabaseContext.SkillsLearning.Select(type => type.Name);
        return View(filteredGames);
    }
}
