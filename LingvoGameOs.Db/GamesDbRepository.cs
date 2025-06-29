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

		public async Task<List<Game>> GetAllAsync()
		{
			return await databaseContext.Games
				.Include(g => g.SkillsLearning)
				.Include(g => g.GamePlatform)
				.ToListAsync();
		}

		public async Task<Game?> TryGetByIdAsync(int idGame)
		{
			return await databaseContext.Games
				.Include(g => g.SkillsLearning)
				.Include(g => g.LanguageLevel)
				.Include(g => g.GamePlatform)
				.Include(g => g.Players)
				.Include(g => g.Author)
				.ThenInclude(a => a.DevGames)
				.FirstOrDefaultAsync(game => game.Id == idGame);
		}

		public async Task AddAsync(Game newGame)
		{
			await databaseContext.Games.AddAsync(newGame);
			await databaseContext.SaveChangesAsync();
		}

		public async Task RemoveAsync(Game game)
		{
			databaseContext.Games.Remove(game);
			await databaseContext.SaveChangesAsync();
		}
		
		public async Task AddPlayerToGameHistoryAsync(Game game, User player)
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
            await databaseContext.SaveChangesAsync();
		}

		public async Task<List<Game>?> TryGetUserGameHistoryAsync(User user)
		{
			var existingUser = await databaseContext.Users
				.Include(u => u.PlayerGames)
				.FirstOrDefaultAsync(u => u.Id == user.Id);
			if (existingUser == null)
				return null;
			return existingUser.PlayerGames;
		}

        public async Task<List<Game>?> TryGetUserDevGamesAsync(User user)
		{
            var existingUser = await databaseContext.Users
                .Include(u => u.DevGames!)
                .ThenInclude(g => g.GamePlatform)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
                return null;
            return existingUser.DevGames;
        }

		public async Task ChangeGameUrl(string newGameUrl, Game game)
		{
			game.GameURL = newGameUrl;
			await databaseContext.SaveChangesAsync();
		}
    }
}
