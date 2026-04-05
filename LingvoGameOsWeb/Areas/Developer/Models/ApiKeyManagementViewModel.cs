namespace LingvoGameOs.Areas.Developer.Models
{
    public class ApiKeyManagementViewModel
    {
        public bool HasApiKey { get; set; }
        public Guid ApiKeyId { get; set; }
        public DateTimeOffset? ApiKeyCreatedAt { get; set; }
        public DateTimeOffset? ApiKeyExpiresAt { get; set; }
        public DateTimeOffset? ApiKeyLastUsed { get; set; }
        public bool IsApiKeyActive { get; set; }
    }
}
