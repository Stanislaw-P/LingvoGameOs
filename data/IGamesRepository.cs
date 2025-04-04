using LingvoGameOs.data.Models;

namespace LingvoGameOs.data
{
	public interface IGamesRepository
	{
		void Add(Game newGame);
		List<Game> GetAll();
		void Remove(Game game);
		Game? TryGetById(int idGame);
	}
}