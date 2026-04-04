using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LingvoGameOs.Db.Services
{
    public class ApiKeyService : IApiKeyService
    {
        readonly DatabaseContext _dbContext;
        readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(DatabaseContext databaseContext, ILogger<ApiKeyService> logger)
        {
            _dbContext = databaseContext;
            _logger = logger;
        }

        public async Task<ApiKey> CreateApiKeyAsync(string userId, string name)
        {
            // Генерируем уникальный API ключ
            var (apiKeyString, keyHash) = GenerateApiKeyWithHash();

            // Создаем запись в БД
            var apiKey = new ApiKey
            {
                UserId = userId,
                KeyHash = keyHash,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            _dbContext.ApiKeys.Add(apiKey);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"API key created for user {userId}. Key ID: {apiKey.Id}");

            // Сохраняем ID ключа для использования в дальнейшем
            apiKey.KeyHash = apiKeyString; // Временное хранилище для возврата ключа

            return apiKey;
        }

        private (string apiKey, string hash) GenerateApiKeyWithHash()
        {
            // Генерируем 32-байтный ключ
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            // Формат: cat_{id}_{timestamp}_{random}
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var randomPart = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            var apiKey = $"cat_{timestamp}_{randomPart}";

            // Хешируем ключ для хранения в БД
            var hash = HashApiKey(apiKey);

            return (apiKey, hash);
        }

        private string HashApiKey(string apiKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(apiKey);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            var keyHash = HashApiKey(apiKey);

            var keyRecord = await _dbContext.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.KeyHash == keyHash);

            if (keyRecord == null)
            {
                _logger.LogWarning($"API key not found in database");
                return false;
            }

            // Проверяем активность
            if (!keyRecord.IsActive)
            {
                _logger.LogWarning($"API key {keyRecord.Id} is inactive");
                return false;
            }

            // Проверяем, существует ли пользователь
            if (keyRecord.User == null || string.IsNullOrWhiteSpace(keyRecord.UserId))
            {
                _logger.LogWarning($"User for API key {keyRecord.Id} is not active");
                return false;
            }

            // Обновляем время последнего использования
            keyRecord.LastUsedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"API key {keyRecord.Id} validated successfully for user {keyRecord.UserId}");

            return true;
        }

        public async Task<ApiKey?> GetApiKeyByKeyAsync(string apiKey)
        {
            var keyHash = HashApiKey(apiKey);

            return await _dbContext.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.KeyHash == keyHash);
        }

        public async Task<User?> GetUserByApiKeyAsync(string apiKey)
        {
            var keyHash = HashApiKey(apiKey);

            var keyRecord = await _dbContext.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.KeyHash == keyHash && k.IsActive);

            return keyRecord?.User;
        }

        public async Task<List<ApiKey>> GetUserApiKeysAsync(string userId)
        {
            return await _dbContext.ApiKeys
                .Where(k => k.UserId == userId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteApiKeyAsync(Guid apiKeyId, string userId)
        {
            var apiKey = await _dbContext.ApiKeys
                .FirstOrDefaultAsync(k => k.Id == apiKeyId && k.UserId == userId);

            if (apiKey == null)
                return false;

            _dbContext.ApiKeys.Remove(apiKey);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"API key {apiKeyId} deleted for user {userId}");

            return true;
        }

        public async Task<bool> RevokeApiKeyAsync(Guid apiKeyId, string userId)
        {
            var apiKey = await _dbContext.ApiKeys
                .FirstOrDefaultAsync(k => k.Id == apiKeyId && k.UserId == userId);

            if (apiKey == null)
                return false;

            apiKey.IsActive = false;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"API key {apiKeyId} revoked for user {userId}");

            return true;
        }
    }
}
