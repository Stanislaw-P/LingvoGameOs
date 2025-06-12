using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using LingvoGameOs.Db;


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

    public async Task<IActionResult> News()
    {
        return View();
    }
}
