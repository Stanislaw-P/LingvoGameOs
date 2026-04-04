using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db.Services
{
    public interface IApiKeyService
    {
        Task<ApiKey> CreateApiKeyAsync(string userId, string name);
        Task<bool> DeleteApiKeyAsync(Guid apiKeyId, string userId);
        Task<ApiKey?> GetApiKeyByKeyAsync(string apiKey);
        Task<User?> GetUserByApiKeyAsync(string apiKey);
        Task<List<ApiKey>> GetUserApiKeysAsync(string userId);
        Task<bool> ValidateApiKeyAsync(string apiKey);
        Task<bool> RevokeApiKeyAsync(Guid apiKeyId, string userId);
    }
}