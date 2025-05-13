using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
	public class GamesDbRepository : IGamesRepository
	{
		readonly NewDatabaseContext newDatabaseContext;

		public GamesDbRepository(NewDatabaseContext newDatabaseContext)
		{
			this.newDatabaseContext = newDatabaseContext;
		}

		public List<Game> GetAll()
		{
			return newDatabaseContext.Games.ToList();
		}

		public Game? TryGetById(int idGame)
		{
			return newDatabaseContext.Games
				.Include(g => g.GameTypes)
				.Include(g => g.LanguageLevel)
				.Include(g => g.GamePlatform)
				//.Include(g => g.Author)
				//.ThenInclude(a=>a.DevGames)
				.FirstOrDefault(game => game.Id == idGame);
		}

		public void Add(Game newGame)
		{
			newDatabaseContext.Games.Add(newGame);
			newDatabaseContext.SaveChanges();
		}

		public void Remove(Game game)
		{
			newDatabaseContext.Games.Remove(game);
			newDatabaseContext.SaveChanges();
		}
	}
}
