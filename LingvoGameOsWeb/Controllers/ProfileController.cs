using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
