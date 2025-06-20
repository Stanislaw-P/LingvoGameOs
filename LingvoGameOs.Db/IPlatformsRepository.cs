using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IPlatformsRepository
    {
        Task<Platform?> GetExistingPlatformAsync(string nameSelectedPlatform);
    }
}