using System.Globalization;
using LingvoGameOs.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers;

public sealed class LanguageController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Primary(string culture, string? returnUrl)
    {
        if (!SupportedLanguages.Contains(culture))
        {
            return BadRequest();
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            BuildCookieOptions());

        var currentPrimary = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var currentSecondary = Request.Cookies[SupportedLanguages.SecondLanguageCookieName];
        if (string.Equals(currentSecondary, culture, StringComparison.OrdinalIgnoreCase))
        {
            Response.Cookies.Append(
                SupportedLanguages.SecondLanguageCookieName,
                string.Equals(currentPrimary, culture, StringComparison.OrdinalIgnoreCase)
                    ? SupportedLanguages.Disabled
                    : currentPrimary,
                BuildCookieOptions());
        }

        return ReturnToLocalPage(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Secondary(string culture, string? returnUrl)
    {
        if (!SupportedLanguages.Contains(culture) &&
            !string.Equals(culture, SupportedLanguages.Disabled, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        Response.Cookies.Append(
            SupportedLanguages.SecondLanguageCookieName,
            culture.ToLowerInvariant(),
            BuildCookieOptions());

        return ReturnToLocalPage(returnUrl);
    }

    private CookieOptions BuildCookieOptions() => new()
    {
        Expires = DateTimeOffset.UtcNow.AddYears(1),
        HttpOnly = true,
        IsEssential = true,
        Path = "/",
        SameSite = SameSiteMode.Lax,
        Secure = Request.IsHttps
    };

    private IActionResult ReturnToLocalPage(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction("Index", "Home")!;
}
