using System.Globalization;
using System.Resources;
using LingvoGameOs.Localization;

namespace LingvoGameOs.Helpers;

public sealed class SecondLocalizer(IHttpContextAccessor httpContextAccessor)
{
    private readonly ResourceManager _resources = new(
        $"{typeof(SharedResource).Assembly.GetName().Name}.Resources.{nameof(SharedResource)}",
        typeof(SharedResource).Assembly);

    public CultureInfo? Culture
    {
        get
        {
            var primary = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var requested = httpContextAccessor.HttpContext?
                .Request.Cookies[SupportedLanguages.SecondLanguageCookieName];

            if (string.Equals(requested, SupportedLanguages.Disabled, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (SupportedLanguages.Contains(requested) &&
                !string.Equals(requested, primary, StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.GetCultureInfo(requested!);
            }

            var fallback = string.Equals(primary, "os", StringComparison.OrdinalIgnoreCase)
                ? "ru"
                : "os";

            return CultureInfo.GetCultureInfo(fallback);
        }
    }

    public string CultureCode => Culture?.TwoLetterISOLanguageName.ToUpperInvariant() ?? "—";

    public string this[string key]
    {
        get
        {
            var culture = Culture;
            return culture is null ? string.Empty : _resources.GetString(key, culture) ?? key;
        }
    }
}
