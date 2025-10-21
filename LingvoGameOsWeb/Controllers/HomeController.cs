using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using LingvoGameOs.Db;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using LingvoGameOs.Db.Models;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    readonly IGamesRepository _gamesRepository;
    readonly IFavoriteGamesRepository _favoriteGamesRepository;
    readonly UserManager<User> _userManager;
    readonly ISkillsLearningRepository _skillsLearningRepository;

    public HomeController(IGamesRepository gamesRepository, IFavoriteGamesRepository favoriteGamesRepository, UserManager<User> userManager, ISkillsLearningRepository skillsLearningRepository)
    {
        _gamesRepository = gamesRepository;
        _favoriteGamesRepository = favoriteGamesRepository;
        _userManager = userManager;
        _skillsLearningRepository = skillsLearningRepository;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var games = await _gamesRepository.GetAllAsync();

        var gamesViewModel = new List<GameViewModel>();

        foreach (var game in games)
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

        var skillsLearning = await _skillsLearningRepository.GetAllAsync();
        ViewBag.SkillsLearning = skillsLearning.Select(sk => sk.Name).ToList();

        return View(gamesViewModel);
    }

    public async Task<IActionResult> Search(string gameName)
    {
        var games = await _gamesRepository.GetAllAsync();

        if (gameName != null)
        {
            games = games.Where(game => Regex.IsMatch(game.Title, gameName, RegexOptions.IgnoreCase)).ToList();
            ViewBag.GameName = gameName;
        }

        return PartialView("_GamesListPartial", games);
    }

    public async Task<IActionResult> FullGamesList()
    {
        var games = await _gamesRepository.GetAllAsync();
        return PartialView("_GamesListPartial", games);
    }

    public async Task<IActionResult> News()
    {
        return View();
    }

    public async Task<IActionResult> Games()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var games = await _gamesRepository.GetAllAsync();

        var gamesViewModel = new List<GameViewModel>();

        foreach (var game in games)
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

        var skillsLearning = await _skillsLearningRepository.GetAllAsync();
        ViewBag.SkillsLearning = skillsLearning.Select(sk => sk.Name).ToList();

        return View(gamesViewModel);
    }

    public async Task<IActionResult> Categories()
    {
        var games = await _gamesRepository.GetAllAsync();
        
        var skillsLearning = await _skillsLearningRepository.GetAllAsync();
        ViewBag.SkillsLearning = skillsLearning.Select(sk => sk.Name).ToList();

        return View(games);
    }

    public async Task<IActionResult> CategoryGames(string category)
    {
        var games = await _gamesRepository.GetAllAsync();
        var filteredGames = games.Where(game =>
            game.SkillsLearning != null &&
            game.SkillsLearning.Any(skill =>
                skill.Name.Equals(category, StringComparison.OrdinalIgnoreCase)
            )
        ).ToList();

        ViewBag.Category = category;
        
        var skillsLearning = await _skillsLearningRepository.GetAllAsync();
        ViewBag.SkillsLearning = skillsLearning.Select(sk => sk.Name).ToList();
        
        return View(filteredGames);
    }
}
