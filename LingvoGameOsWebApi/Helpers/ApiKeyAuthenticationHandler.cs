using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LingvoGameOsWebApi.Helpers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyService _apiKeyService;
        private const string ApiKeyHeaderName = "X-API-Key";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IApiKeyService apiKeyService)
            : base(options, logger, encoder)
        {
            _apiKeyService = apiKeyService;
        }

        // Основной метод аутентификации
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Проверяем наличие заголовка
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            // Валидируем API ключ
            var isValid = await _apiKeyService.ValidateApiKeyAsync(providedApiKey);

            if (!isValid)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            // Получаем пользователя
            var user = await _apiKeyService.GetUserByApiKeyAsync(providedApiKey);

            if (user == null)
            {
                return AuthenticateResult.Fail("User not found");
            }

            // Создаем claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("AuthenticationMethod", "ApiKey")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        // Обработка ошибки аутентификации (401)
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            var error = new
            {
                error = "unauthorized",
                message = "Invalid or missing API Key",
                statusCode = 401
            };

            await Response.WriteAsJsonAsync(error);
        }

        // Обработка запрета доступа (403)
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = "application/json";

            var error = new
            {
                error = "forbidden",
                message = "Access denied",
                statusCode = 403
            };

            await Response.WriteAsJsonAsync(error);
        }
    }
}
