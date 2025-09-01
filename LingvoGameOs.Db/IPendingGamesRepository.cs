using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IPendingGamesRepository
    {
        Task AddAsync(PendingGame newGame);
        Task ChangeGameUrlAsync(string newGameUrl, PendingGame game);
        Task<List<PendingGame>> GetAllAsync();
        Task RemoveAsync(PendingGame game);
        Task<PendingGame?> TryGetByIdAsync(int idGame);
        Task ChangeImagesAsync(string coverImgPath, List<string> imgPaths, PendingGame game);
        Task UpdateAsync(PendingGame updatedGame);
        Task<Game> PublishAsync(PendingGame pendingGame);
        Task<List<PendingGame>?> TryGetUserDevGamesAsync(User user);
    }
}