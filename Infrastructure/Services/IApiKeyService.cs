using LingvoGameOs.Db.Models;

namespace LingvoGameOsWebApi.Helpers
{
    public interface IApiKeyService
    {
        Task<ApiKey> CreateApiKeyAsync(string userId, string name, int? expirationDays = null);
        Task<bool> DeleteApiKeyAsync(Guid apiKeyId, string userId);
        Task<ApiKey?> GetApiKeyByKeyAsync(string apiKey);
        Task<User?> GetUserByApiKeyAsync(string apiKey);
        Task<bool> ValidateApiKeyAsync(string apiKey);
        Task<bool> RevokeApiKeyAsync(Guid apiKeyId, string userId);
    }
}