using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using LingvoGameOs.Db;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    readonly IGamesRepository gamesRepository;
    readonly DatabaseContext databaseContext;

    public HomeController(IGamesRepository gamesRepository, DatabaseContext databaseContext)
    {
        this.gamesRepository = gamesRepository;
        this.databaseContext = databaseContext;
    }

    public IActionResult Index()
    {
        var games = gamesRepository.GetAll();
        ViewBag.GameTypes = databaseContext.GameTypes.Select(type => type.Name);
        return View(games);
    }

    // тестирование класса пользователей
    public string Usser()
    {
        UserViewModel user = new PlayerUserViewModel("ddkhugaev@gmail.com", "1234", "Давид", "Хугаев");
        return user.ToString();
        //return View();
    }
}
