using LingvoGameOs.Db.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LingvoGameOsWebApi.Authentication;

public sealed class DeveloperApiKeyAuthenticationHandler
    : AuthenticationHandler<DeveloperApiKeyAuthenticationOptions>
{
    readonly IApiKeyService _apiKeyService;

    public DeveloperApiKeyAuthenticationHandler(
        IOptionsMonitor<DeveloperApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyService apiKeyService)
        : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(
                DeveloperApiKeyAuthenticationDefaults.HeaderName,
                out var values))
        {
            return AuthenticateResult.NoResult();
        }

        if (values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
            return AuthenticateResult.Fail("Invalid API key header.");

        var validatedKey = await _apiKeyService.ValidateAsync(
            values[0]!,
            Context.RequestAborted);

        if (validatedKey == null)
            return AuthenticateResult.Fail("Invalid, expired or revoked API key.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, validatedKey.User.Id),
            new(
                ClaimTypes.Name,
                validatedKey.User.UserName ?? validatedKey.User.Email ?? validatedKey.User.Id),
            new(
                DeveloperApiKeyAuthenticationDefaults.ApiKeyIdClaim,
                validatedKey.ApiKeyId.ToString()),
        };

        if (!string.IsNullOrWhiteSpace(validatedKey.User.Email))
            claims.Add(new Claim(ClaimTypes.Email, validatedKey.User.Email));

        claims.AddRange(validatedKey.Scopes.Select(scope =>
            new Claim(DeveloperApiKeyAuthenticationDefaults.ScopeClaim, scope)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Response.WriteAsJsonAsync(new
        {
            error = "unauthorized",
            message = $"Передайте действующий API-ключ в заголовке {DeveloperApiKeyAuthenticationDefaults.HeaderName}."
        });
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Response.WriteAsJsonAsync(new
        {
            error = "forbidden",
            message = "У API-ключа недостаточно прав для этой операции."
        });
    }
}
