using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models;

public sealed class ApiKeyManagementViewModel
{
    public IReadOnlyList<ApiKeyListItemViewModel> ApiKeys { get; init; } = [];
    public string? CreatedSecret { get; init; }
    public int MaxActiveKeys { get; init; }

    [Required(ErrorMessage = "Укажи название ключа.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Название должно содержать от 1 до 100 символов.")]
    [Display(Name = "Название ключа")]
    public string NewKeyName { get; set; } = "Мой API-ключ";
}

public sealed record ApiKeyListItemViewModel(
    Guid Id,
    string Name,
    string KeyPrefix,
    string Scopes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastUsedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? RevokedAt,
    bool IsActive);
