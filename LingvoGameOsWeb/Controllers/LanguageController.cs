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

            var returnUrl = Request.Headers.Referer.ToString();
            return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }

    public IActionResult Second(string culture)
        {
            Response.Cookies.Append(
                "SecondLanguage",
                culture,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                }
            );
            var returnUrl = Request.Headers.Referer.ToString();
            return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }
    }
}
