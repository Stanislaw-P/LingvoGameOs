using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
	public class GamesDbRepository : IGamesRepository
	{
		readonly DatabaseContext databaseContext;

		public GamesDbRepository(DatabaseContext newDatabaseContext)
		{
			this.databaseContext = newDatabaseContext;
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
				.Include(g => g.Players)
				.Include(g => g.Author)
				.ThenInclude(a => a.DevGames)
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
		
		public void AddPlayerToGameHistory(Game game, User player)
		{
			if (game == null || player == null)
				return;
			if (game.Players == null)
				game.Players = new List<User> { player };
			else
			{
				if(!game.Players.Any(u => u.Id == player.Id))
                    game.Players.Add(player);
            }
            databaseContext.SaveChanges();
		}

		public List<Game>? TryGetUserGameHistory(User user)
		{
			var existingUser = databaseContext.Users
				.Include(u => u.PlayerGames)
				.FirstOrDefault(u => u.Id == user.Id);
			if (existingUser == null)
				return null;
			return existingUser.PlayerGames;
		}
	}
}
