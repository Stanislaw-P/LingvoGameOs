using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult Index(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            string returnUrl = Request.Headers.Referer.ToString();
            return Redirect(returnUrl);
        }
    }
}
