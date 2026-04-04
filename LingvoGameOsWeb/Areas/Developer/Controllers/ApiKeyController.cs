using LingvoGameOs.Areas.Developer.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Db.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Areas.Developer.Controllers
{
    [Area("Developer")]
    [Authorize(Roles = Constants.DevRoleName)]
    public class ApiKeyController : Controller
    {
        readonly IApiKeyService _apiKeyService;
        readonly UserManager<User> _userManager;
        readonly ILogger<ApiKeyController> _logger;

        public ApiKeyController(IApiKeyService apiKeyService, UserManager<User> userManager, ILogger<ApiKeyController> logger)
        {
            _apiKeyService = apiKeyService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var apiKeys = await _apiKeyService.GetUserApiKeysAsync(user.Id);
            var activeKey = apiKeys.FirstOrDefault(k => k.IsActive);

            var model = new ApiKeyManagementViewModel
            {
                HasApiKey = activeKey != null,
                ApiKeyCreatedAt = activeKey?.CreatedAt,
                ApiKeyLastUsed = activeKey?.LastUsedAt,
                IsApiKeyActive = activeKey?.IsActive ?? false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { error = "Пользователь не найден" });

            try
            {
                // Проверяем, есть ли уже активный ключ
                var existingKeys = await _apiKeyService.GetUserApiKeysAsync(user.Id);
                if (existingKeys.Any(k => k.IsActive))
                {
                    return BadRequest(new { error = "У вас уже есть активный API ключ" });
                }

                var apiKey = await _apiKeyService.CreateApiKeyAsync(
                    userId: user.Id,
                    name: "Developer API Key"
                );

                return Ok(new { apiKey = apiKey.KeyHash, apiKeyId = apiKey.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating API key for user {UserId}", user.Id);
                return StatusCode(500, new { error = "Ошибка при создании API ключа" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Regenerate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { error = "Пользователь не найден" });

            try
            {
                // Отзываем все старые ключи
                var existingKeys = await _apiKeyService.GetUserApiKeysAsync(user.Id);
                foreach (var key in existingKeys)
                {
                    await _apiKeyService.RevokeApiKeyAsync(key.Id, user.Id);
                }

                // Создаем новый ключ
                var apiKey = await _apiKeyService.CreateApiKeyAsync(
                     userId: user.Id,
                     name: "Developer API Key"
                 );

                return Ok(new { apiKey = apiKey.KeyHash, apiKeyId = apiKey.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating API key for user {UserId}", user.Id);
                return StatusCode(500, new { error = "Ошибка при регенерации API ключа" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { error = "Пользователь не найден" });

            try
            {
                var existingKeys = await _apiKeyService.GetUserApiKeysAsync(user.Id);
                foreach (var key in existingKeys)
                {
                    if (key.IsActive)
                    {
                        await _apiKeyService.RevokeApiKeyAsync(key.Id, user.Id);
                    }
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking API key for user {UserId}", user.Id);
                return StatusCode(500, new { error = "Ошибка при деактивации API ключа" });
            }
        }
    }
}
