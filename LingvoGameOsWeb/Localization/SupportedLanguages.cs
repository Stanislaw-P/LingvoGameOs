namespace LingvoGameOs.Localization;

public static class SupportedLanguages
{
    public const string Default = "ru";
    public const string Disabled = "off";
    public const string SecondLanguageCookieName = "LingvoGameOs.SecondLanguage";

    public static readonly string[] All = ["ru", "os", "en"];

    public static bool Contains(string? culture) =>
        !string.IsNullOrWhiteSpace(culture) &&
        All.Contains(culture, StringComparer.OrdinalIgnoreCase);
}
