using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
	public IActionResult Index()
    {

        return View();
    }

    // тестирование класса пользователей
    public string Usser()
    {
        UserViewModel user = new PlayerUserViewModel("ddkhugaev@gmail.com", "1234", "Давид", "Хугаев");
        return user.ToString();
        //return View();
    }
}
