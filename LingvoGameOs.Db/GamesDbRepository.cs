using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
	public class GamesDbRepository : IGamesRepository
	{
		readonly DatabaseContext databaseContext;

		public GamesDbRepository(DatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public List<Game> GetAll()
		{
			return databaseContext.Games.ToList();
		}

		public Game? TryGetById(int idGame)
		{
			return databaseContext.Games
				.Include(g => g.GameTypes)
				.Include(g => g.LanguageLevel)
				.Include(g => g.GamePlatform)
				.Include(g => g.Author)
				.FirstOrDefault(game => game.Id == idGame);
		}

		public void Add(Game newGame)
		{
			databaseContext.Games.Add(newGame);
			databaseContext.SaveChanges();
		}

		public void Remove(Game game)
		{
			databaseContext.Games.Remove(game);
			databaseContext.SaveChanges();
		}
	}
}
