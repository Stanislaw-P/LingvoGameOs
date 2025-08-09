using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IPendingGamesRepository
    {
        Task AddAsync(PendingGame newGame);
        Task ChangeGameUrl(string newGameUrl, PendingGame game);
        Task<List<PendingGame>> GetAllAsync();
        Task RemoveAsync(PendingGame game);
        Task<PendingGame?> TryGetByIdAsync(int idGame);
    }
}