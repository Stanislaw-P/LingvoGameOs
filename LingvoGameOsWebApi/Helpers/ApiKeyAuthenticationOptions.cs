using Microsoft.AspNetCore.Authentication;

namespace LingvoGameOsWebApi.Helpers
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
    }
}
