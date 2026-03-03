using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("PageNotFound"); // Ищет Views/Shared/PageNotFound.cshtml
        }

        [Route("error/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("Error");
        }
    }
}