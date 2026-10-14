using LingvoGameOs.Db.Models;
using LingvoGameOs.Db.Services;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers;

[Authorize]
public sealed class ApiKeyController : Controller
{
    readonly IApiKeyService _apiKeyService;
    readonly UserManager<User> _userManager;
    readonly ILogger<ApiKeyController> _logger;

    public ApiKeyController(
        IApiKeyService apiKeyService,
        UserManager<User> userManager,
        ILogger<ApiKeyController> logger)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        return View(await BuildViewModelAsync(user.Id, cancellationToken: cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ApiKeyManagementViewModel input,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        if (!ModelState.IsValid)
        {
            return View(
                "Index",
                await BuildViewModelAsync(
                    user.Id,
                    input.NewKeyName,
                    cancellationToken: cancellationToken));
        }

        try
        {
            var created = await _apiKeyService.CreateAsync(
                user.Id,
                input.NewKeyName,
                cancellationToken);

            return View(
                "Index",
                await BuildViewModelAsync(
                    user.Id,
                    input.NewKeyName,
                    created.Secret,
                    cancellationToken));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(input.NewKeyName), exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create API key for user {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "Не удалось создать API-ключ. Попробуй ещё раз.");
        }

        return View(
            "Index",
            await BuildViewModelAsync(
                user.Id,
                input.NewKeyName,
                cancellationToken: cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var revoked = await _apiKeyService.RevokeAsync(id, user.Id, cancellationToken);
        if (!revoked)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    async Task<ApiKeyManagementViewModel> BuildViewModelAsync(
        string userId,
        string? newKeyName = null,
        string? createdSecret = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var apiKeys = await _apiKeyService.GetForUserAsync(userId, cancellationToken);

        return new ApiKeyManagementViewModel
        {
            ApiKeys = apiKeys.Select(key => new ApiKeyListItemViewModel(
                key.Id,
                key.Name,
                key.KeyPrefix,
                key.Scopes,
                key.CreatedAt,
                key.LastUsedAt,
                key.ExpiresAt,
                key.RevokedAt,
                key.RevokedAt == null && (key.ExpiresAt == null || key.ExpiresAt > now)))
                .ToList(),
            CreatedSecret = createdSecret,
            MaxActiveKeys = ApiKeyService.MaxActiveKeysPerUser,
            NewKeyName = newKeyName ?? "Мой API-ключ",
        };
    }
}
