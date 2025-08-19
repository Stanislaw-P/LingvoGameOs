using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
    public class PendingGamesDbRepository : IPendingGamesRepository
    {
        readonly DatabaseContext databaseContext;

        public PendingGamesDbRepository(DatabaseContext newDatabaseContext)
        {
            this.databaseContext = newDatabaseContext;
        }

        public async Task<List<PendingGame>> GetAllAsync()
        {
            return await databaseContext.PendingGames
                .Include(g => g.SkillsLearning)
                .Include(g => g.GamePlatform)
                .Include(g => g.Author)
                .ToListAsync();
        }

        public async Task<PendingGame?> TryGetByIdAsync(int idGame)
        {
            return await databaseContext.PendingGames
                .Include(g => g.SkillsLearning)
                .Include(g => g.LanguageLevel)
                .Include(g => g.GamePlatform)
                .Include(g => g.Author)
                .ThenInclude(a => a.DevGames)
                .FirstOrDefaultAsync(game => game.Id == idGame);
        }

        public async Task AddAsync(PendingGame newGame)
        {
            await databaseContext.PendingGames.AddAsync(newGame);
            await databaseContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(PendingGame game)
        {
            databaseContext.PendingGames.Remove(game);
            await databaseContext.SaveChangesAsync();
        }

        public async Task ChangeGameUrlAsync(string newGameUrl, PendingGame game)
        {
            game.GameURL = newGameUrl;
            await databaseContext.SaveChangesAsync();
        }

        public async Task ChangeImagesAsync(string coverImgPath, List<string> imgPaths, PendingGame game)
        {
            game.CoverImagePath = coverImgPath;
            game.ImagesPaths = imgPaths;
            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PendingGame updatedGame)
        {
            PendingGame? existingGame = await TryGetByIdAsync(updatedGame.Id);
            if (existingGame == null)
                throw new InvalidOperationException($"Игра с Id {updatedGame.Id} не найдена :(");

            existingGame.Title = updatedGame.Title;
            existingGame.Description = updatedGame.Description;
            existingGame.Rules = updatedGame.Rules;
           
        }
    }
}
