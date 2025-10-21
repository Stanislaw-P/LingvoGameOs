using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        readonly IFavoriteGamesRepository _favoriteGamesRepository;
        readonly UserManager<User> _userManager;
        readonly IGamesRepository _gamesRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;

        public FavoritesController(IFavoriteGamesRepository favoriteGamesRepository, UserManager<User> userManager, IGamesRepository gamesRepository, ISkillsLearningRepository skillsLearningRepository)
        {
            _favoriteGamesRepository = favoriteGamesRepository;
            _userManager = userManager;
            _gamesRepository = gamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var skillsLearning = await _skillsLearningRepository.GetAllAsync();
            ViewBag.SkillsLearning = skillsLearning.Select(sk => sk.Name).ToList();

            // Получаем все избранные игры пользователя
            var favoritesGamesUser = await _favoriteGamesRepository.GetUserFavoritesAsync(currentUser.Id);
            
            var gamesViewModel = new List<GameViewModel>();

            foreach (var game in favoritesGamesUser)
            {
                gamesViewModel.Add(new GameViewModel
                {
                    Id = game.Id,
                    Title = game.Title,
                    Author = game.Author,
                    CoverImagePath = game.CoverImagePath,
                    Description = game.Description,
                    GameFolderName = game.GameFolderName,
                    GameFilePath = game.GameFilePath,
                    GamePlatform = game.GamePlatform,
                    ImagesPaths = game.ImagesPaths,
                    VideoUrl = game.VideoUrl,
                    LanguageLevel = game.LanguageLevel,
                    PublicationDate = game.PublicationDate,
                    SkillsLearning = game.SkillsLearning,
                    RaitingPlayers = game.RaitingPlayers,
                    FavoritesCount = await _favoriteGamesRepository.GetGameFavoritesCountAsync(game.Id),
                    IsFavorite = currentUser != null &&
                               await _favoriteGamesRepository.IsGameInFavoritesAsync(currentUser.Id, game.Id),
                    IsActive = game.IsActive
                });
            }
            return View(gamesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound(new
                {
                    success = false,
                    message = $"Игра с id: {gameId} не найдена."
                });
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized(new
                {
                    success = false,
                    message = "Необходимо войти в аккаунт."
                });
            
            bool result = await _favoriteGamesRepository.AddAsync(currentUser.Id, gameId);
            if(result)
                return Ok();
            return BadRequest("Не удалось сохранить игру. Попробуйте позже или обновите страницу.");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound(new
                {
                    success = false,
                    message = $"Игра с id: {gameId} не найдена."
                });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized(new
                {
                    success = false,
                    message = "Необходимо войти в аккаунт."
                });

            bool result = await _favoriteGamesRepository.RemoveFromFavoritesAsync(currentUser.Id, gameId);
            if (result)
                return Ok();
            return BadRequest("Не удалось удалить игру из избранного. Попробуйте позже или обновите страницу.");
        }
    }
}
