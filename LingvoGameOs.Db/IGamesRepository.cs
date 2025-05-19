using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
	public interface IGamesRepository
	{
		void Add(Game newGame);
		List<Game> GetAll();
		void Remove(Game game);
		Game? TryGetById(int idGame);
		void AddPlayerToGameHistory(Game game, User player);
		List<Game>? TryGetUserGameHistory(User user);
		List<Game>? TryGetUserDevGames(User user);
    }
}