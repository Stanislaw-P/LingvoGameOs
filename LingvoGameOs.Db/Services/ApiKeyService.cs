using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LingvoGameOs.Db.Services;

public sealed class ApiKeyService : IApiKeyService
{
    public const string GamesReadScope = "games:read";
    public const int MaxActiveKeysPerUser = 5;

    static readonly TimeSpan LastUsedWriteInterval = TimeSpan.FromMinutes(10);
    static readonly TimeSpan DefaultLifetime = TimeSpan.FromDays(365);

    readonly DatabaseContext _dbContext;
    readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(DatabaseContext dbContext, ILogger<ApiKeyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CreatedApiKey> CreateAsync(
        string userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        name = name.Trim();
        if (name.Length is < 1 or > 100)
            throw new ArgumentException("Название ключа должно содержать от 1 до 100 символов.", nameof(name));

        var now = DateTimeOffset.UtcNow;
        var activeKeyCount = await _dbContext.ApiKeys.CountAsync(
            key => key.UserId == userId
                && key.RevokedAt == null
                && (key.ExpiresAt == null || key.ExpiresAt > now),
            cancellationToken);

        if (activeKeyCount >= MaxActiveKeysPerUser)
            throw new InvalidOperationException($"Можно создать не более {MaxActiveKeysPerUser} активных API-ключей.");

        var secret = GenerateSecret();
        var apiKey = new ApiKey
        {
            UserId = userId,
            Name = name,
            KeyPrefix = secret[..Math.Min(secret.Length, 12)],
            KeyHash = Hash(secret),
            Scopes = GamesReadScope,
            CreatedAt = now,
            ExpiresAt = now.Add(DefaultLifetime),
        };

        _dbContext.ApiKeys.Add(apiKey);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "API key {ApiKeyId} created for user {UserId}",
            apiKey.Id,
            userId);

        return new CreatedApiKey(apiKey, secret);
    }

    public async Task<IReadOnlyList<ApiKey>> GetForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiKeys
            .AsNoTracking()
            .Where(key => key.UserId == userId)
            .OrderByDescending(key => key.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ValidatedApiKey?> ValidateAsync(
        string secret,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(secret)
            || secret.Length > 128
            || !secret.StartsWith("lgo_", StringComparison.Ordinal))
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var hash = Hash(secret);
        var apiKey = await _dbContext.ApiKeys
            .AsNoTracking()
            .Include(key => key.User)
            .SingleOrDefaultAsync(
                key => key.KeyHash == hash
                    && key.RevokedAt == null
                    && (key.ExpiresAt == null || key.ExpiresAt > now),
                cancellationToken);

        if (apiKey?.User == null)
            return null;

        if (apiKey.LastUsedAt == null || apiKey.LastUsedAt < now - LastUsedWriteInterval)
        {
            await _dbContext.ApiKeys
                .Where(key => key.Id == apiKey.Id)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(key => key.LastUsedAt, now),
                    cancellationToken);
        }

        var scopes = apiKey.Scopes.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new ValidatedApiKey(apiKey.Id, apiKey.User, scopes);
    }

    public async Task<bool> RevokeAsync(
        Guid apiKeyId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var revokedAt = DateTimeOffset.UtcNow;
        var changed = await _dbContext.ApiKeys
            .Where(key => key.Id == apiKeyId && key.UserId == userId && key.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(key => key.RevokedAt, revokedAt),
                cancellationToken);

        if (changed > 0)
        {
            _logger.LogInformation(
                "API key {ApiKeyId} revoked for user {UserId}",
                apiKeyId,
                userId);
        }

        return changed > 0;
    }

    static string GenerateSecret()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var randomPart = Convert.ToBase64String(randomBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return $"lgo_{randomPart}";
    }

    static string Hash(string secret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToHexString(bytes);
    }
}
