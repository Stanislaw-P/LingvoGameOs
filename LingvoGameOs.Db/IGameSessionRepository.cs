using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IGameSessionRepository
    {
        Task AddAsync(GameSession session);
        Task<GameSession?> TryGetByIdAsync(int id);
        Task<GameSession?> TryGetAsync(string userId, int gameId);
    }
}