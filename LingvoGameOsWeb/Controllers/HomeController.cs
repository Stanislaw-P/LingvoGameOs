using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using LingvoGameOs.Db;
using System.Text.RegularExpressions;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    readonly IGamesRepository gamesRepository;
    readonly DatabaseContext newDatabaseContext;

    public HomeController(IGamesRepository gamesRepository, DatabaseContext newDatabaseContext)
    {
        this.gamesRepository = gamesRepository;
        this.newDatabaseContext = newDatabaseContext;
    }

    public async Task<IActionResult> Index()
    {
        var games = await gamesRepository.GetAllAsync();
        ViewBag.SkillsLearning = newDatabaseContext.SkillsLearning.Select(type => type.Name);
        return View(games);
    }

    public async Task<IActionResult> Search(string gameName)
    {
        var games = await gamesRepository.GetAllAsync();

        if (gameName != null)
        {
            games = games.Where(game => Regex.IsMatch(game.Title, gameName, RegexOptions.IgnoreCase)).ToList();
            ViewBag.GameName = gameName;
        }

       /* Thread.Sleep(2000);*/ // Демонстрация "поиска игр"
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
}
