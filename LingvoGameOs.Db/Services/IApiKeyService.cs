using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db.Services;

public interface IApiKeyService
{
    Task<CreatedApiKey> CreateAsync(
        string userId,
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApiKey>> GetForUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<ValidatedApiKey?> ValidateAsync(
        string secret,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(
        Guid apiKeyId,
        string userId,
        CancellationToken cancellationToken = default);
}

public sealed record CreatedApiKey(ApiKey ApiKey, string Secret);

public sealed record ValidatedApiKey(
    Guid ApiKeyId,
    User User,
    IReadOnlyCollection<string> Scopes);
