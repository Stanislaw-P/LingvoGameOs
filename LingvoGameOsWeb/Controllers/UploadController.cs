using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
