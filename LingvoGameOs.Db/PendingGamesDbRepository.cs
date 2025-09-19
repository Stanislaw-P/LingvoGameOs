using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LingvoGameOs.Db
{
    public class PendingGamesDbRepository : IPendingGamesRepository
    {
        readonly DatabaseContext databaseContext;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;

        public PendingGamesDbRepository(DatabaseContext newDatabaseContext, ISkillsLearningRepository skillsLearningRepository, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository)
        {
            this.databaseContext = newDatabaseContext;
            _skillsLearningRepository = skillsLearningRepository;
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
        }

        public async Task<List<PendingGame>> GetAllAsync()
        {
            return await databaseContext.PendingGames
                .Include(g => g.SkillsLearning)
                .Include(g => g.GamePlatform)
                .Include(g => g.Author)
                .AsSplitQuery()
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
                .AsSplitQuery()
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

        /// <summary>
        /// Метод нужен для сохранения изменений в контексте БД
        /// </summary>
        /// <param name="coverImgPath">Путь к обложке</param>
        /// <param name="imgPaths">Путь к скриншотам</param>
        /// <param name="game">Игра</param>
        /// <returns></returns>
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

            databaseContext.PendingGames.Update(updatedGame);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<Game> PublishAsync(PendingGame pendingGame)
        {
            var newGame = new Game
            {
                Title = pendingGame.Title,
                Description = pendingGame.Description,
                Rules = pendingGame.Rules,
                AuthorId = pendingGame.AuthorId,
                LanguageLevelId = pendingGame.LanguageLevelId, 
                GamePlatformId = pendingGame.GamePlatformId,
                SkillsLearning = pendingGame.SkillsLearning,
                GameFolderName = pendingGame.GameFolderName,
                GameFilePath = pendingGame.GameURL,
                VideoUrl = pendingGame.VideoUrl,
                PublicationDate = DateTimeOffset.UtcNow,
                LastUpdateDate = DateTimeOffset.UtcNow,
            };

            databaseContext.PendingGames.Remove(pendingGame);
            await databaseContext.Games.AddAsync(newGame);
            await databaseContext.SaveChangesAsync();
            return newGame;
        }

        public async Task<List<PendingGame>> TryGetUserDevGamesAsync(User user)
        {
            return await databaseContext.PendingGames
                .Where(g => g.Author.Id == user.Id)
                .Include(g => g.GamePlatform)
                .ToListAsync();
        }
    }
}
