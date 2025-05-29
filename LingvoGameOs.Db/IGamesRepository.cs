using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
	public interface IGamesRepository
	{
		Task AddAsync(Game newGame);
		Task<List<Game>> GetAllAsync();
		Task RemoveAsync(Game game);
		Task<Game?> TryGetByIdAsync(int idGame);
		Task AddPlayerToGameHistoryAsync(Game game, User player);
		Task<List<Game>?> TryGetUserGameHistoryAsync(User user);
		Task<List<Game>?> TryGetUserDevGamesAsync(User user);
    }
}