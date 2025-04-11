using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
	public interface IGamesRepository
	{
		void Add(Game newGame);
		List<Game> GetAll();
		void Remove(Game game);
		Game? TryGetById(int idGame);
	}
}