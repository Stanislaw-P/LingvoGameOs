namespace LingvoGameOsWebApi.Authentication;

public static class DeveloperApiKeyAuthenticationDefaults
{
    public const string Scheme = "DeveloperApiKey";
    public const string HeaderName = "X-API-Key";
    public const string ApiKeyIdClaim = "api_key_id";
    public const string ScopeClaim = "scope";
    public const string Policy = "DeveloperApi";
    public const string GamesReadPolicy = "DeveloperApi.GamesRead";
}
