using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IFavoriteGamesRepository
    {
        Task<bool> AddAsync(string userId, int gameId);
        Task<List<Game>> GetUserFavoritesAsync(string userId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int gameId);
        Task<FavoriteGame?> TryGetFavoriteAsync(string userId, int gameId);
    }
}