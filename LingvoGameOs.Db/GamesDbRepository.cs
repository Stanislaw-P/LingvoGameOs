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
                .Include(g => g.Author)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Game?> TryGetByIdAsync(int idGame)
        {
            return await databaseContext.Games
                .Include(g => g.SkillsLearning)
                .Include(g => g.LanguageLevel)
                .Include(g => g.GamePlatform)
                .Include(g => g.FavoriteGames)
                .Include(g => g.PlayersHistory)
                .Include(g => g.Author)
                .ThenInclude(a => a.DevGames)
                .AsSplitQuery()
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

            if (game.PlayersHistory == null)
            {
                game.PlayersHistory = new List<GameHistory>();
            }

            var existingHistory = game.PlayersHistory
                .FirstOrDefault(gh => gh.UserId == player.Id);

            if (existingHistory != null)
                existingHistory.LastLaunch = DateTimeOffset.UtcNow;
            else
            {
                game.PlayersHistory.Add(new GameHistory
                {
                    GameId = game.Id,
                    UserId = player.Id,
                    LastLaunch = DateTimeOffset.UtcNow
                });
            }

            await databaseContext.SaveChangesAsync();
        }

        public async Task<List<Game>> TryGetUserGameHistoryAsync(User user)
        {
            var existingUser = await databaseContext.Users
                .Include(u => u.GamesHistory!)
                .ThenInclude(gh => gh.Game!)
                .AsSplitQuery()
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return existingUser?.GamesHistory?
                .OrderByDescending(gh => gh.LastLaunch)
                .Select(gh => gh.Game)
                .ToList() ?? new List<Game>();
        }

        public async Task<List<Game>> TryGetUserDevGamesAsync(User user)
        {
            return await databaseContext.Games.Where(g => g.Author.Id == user.Id)
                .Include(g => g.GamePlatform)
                .ToListAsync();
        }

        public async Task ChangeGameUrl(string newGameUrl, Game game)
        {
            game.GameFilePath = newGameUrl;
            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Game updatedGame)
        {
            Game? existingGame = await TryGetByIdAsync(updatedGame.Id);
            if (existingGame == null)
                throw new InvalidOperationException($"Игра с Id {updatedGame.Id} не найдена :(");

            databaseContext.Games.Update(updatedGame);
            await databaseContext.SaveChangesAsync();
        }

        public async Task Deactivate(Game game)
        {
            if (game == null)
                return;

            game.IsActive = false;
            await databaseContext.SaveChangesAsync();
        }

        public async Task Activate(Game game)
        {
            if (game == null)
                return;

            game.IsActive = true;
            await databaseContext.SaveChangesAsync();
        }
    }
}
