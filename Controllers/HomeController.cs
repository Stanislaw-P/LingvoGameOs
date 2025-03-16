using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;

namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    // тестирование класса пользователей
    public string Usser()
    {
        User user = new PlayerUser("ddkhugaev@gmail.com", "1234", "Давид", "Хугаев");
        return user.ToString();
        //return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
