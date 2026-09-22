using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using LingvoGameOs.Db;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;


namespace LingvoGameOs.Controllers;

public class HomeController : Controller
{
    readonly IGamesRepository _gamesRepository;
    readonly IFavoriteGamesRepository _favoriteGamesRepository;
    readonly UserManager<User> _userManager;
    readonly ISkillsLearningRepository _skillsLearningRepository;
    readonly IFileStorage _fileStorage;

    public HomeController(IGamesRepository gamesRepository, IFavoriteGamesRepository favoriteGamesRepository, UserManager<User> userManager, ISkillsLearningRepository skillsLearningRepository, IFileStorage fileStorage)
    {
        _gamesRepository = gamesRepository;
        _favoriteGamesRepository = favoriteGamesRepository;
        _userManager = userManager;
        _skillsLearningRepository = skillsLearningRepository;
        _fileStorage = fileStorage;
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
                CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath!),
                GameFolderName = game.GameFolderName,
                GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                GamePlatform = game.GamePlatform,
                LanguageLevel = game.LanguageLevel,
                PublicationDate = game.PublicationDate,
                SkillsLearning = game.SkillsLearning,
                AverageRaitingPlayers = game.AverageRaitingPlayers,
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
        var currentUser = await _userManager.GetUserAsync(User);

        var games = await _gamesRepository.GetAllAsync();

        if (gameName != null)
        {
            games = games.Where(game => Regex.IsMatch(game.Title, gameName, RegexOptions.IgnoreCase)).ToList();
            ViewBag.GameName = gameName;
        }

        var gamesViewModel = new List<GameViewModel>();

        foreach (var game in games)
        {
            gamesViewModel.Add(new GameViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Author = game.Author,
                CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath),
                GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                GamePlatform = game.GamePlatform,
                ImagesPaths = game.ImagesPaths,
                LanguageLevel = game.LanguageLevel,
                PublicationDate = game.PublicationDate,
                SkillsLearning = game.SkillsLearning,
                AverageRaitingPlayers = game.AverageRaitingPlayers,
                FavoritesCount = await _favoriteGamesRepository.GetGameFavoritesCountAsync(game.Id),
                IsFavorite = currentUser != null &&
                           await _favoriteGamesRepository.IsGameInFavoritesAsync(currentUser.Id, game.Id),
                IsActive = game.IsActive
            });
        }

        return PartialView("_GamesListPartial", gamesViewModel);
    }

    public async Task<IActionResult> FilterGamesAsync(string? platform = null)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var games = await _gamesRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(platform))
        {
            games = games.Where(g => g?.GamePlatform?.Name == platform).ToList();
        }

        var gamesViewModel = new List<GameViewModel>();

        foreach (var game in games)
        {
            gamesViewModel.Add(new GameViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Author = game.Author,
                CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath),
                GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                GamePlatform = game.GamePlatform,
                LanguageLevel = game.LanguageLevel,
                PublicationDate = game.PublicationDate,
                SkillsLearning = game.SkillsLearning,
                AverageRaitingPlayers = game.AverageRaitingPlayers,
                FavoritesCount = await _favoriteGamesRepository.GetGameFavoritesCountAsync(game.Id),
                IsFavorite = currentUser != null &&
                           await _favoriteGamesRepository.IsGameInFavoritesAsync(currentUser.Id, game.Id),
                IsActive = game.IsActive
            });
        }

        return PartialView("_GamesListPartial", gamesViewModel);
    }

    public async Task<IActionResult> FullGamesList()
    {
        var games = await _gamesRepository.GetAllAsync();
        var currentUser = await _userManager.GetUserAsync(User);

        var gamesViewModel = new List<GameViewModel>();

        foreach (var game in games)
        {
            gamesViewModel.Add(new GameViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Author = game.Author,
                CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath!),
                GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                GamePlatform = game.GamePlatform,
                LanguageLevel = game.LanguageLevel,
                PublicationDate = game.PublicationDate,
                SkillsLearning = game.SkillsLearning,
                AverageRaitingPlayers = game.AverageRaitingPlayers,
                FavoritesCount = await _favoriteGamesRepository.GetGameFavoritesCountAsync(game.Id),
                IsFavorite = currentUser != null &&
                           await _favoriteGamesRepository.IsGameInFavoritesAsync(currentUser.Id, game.Id),
                IsActive = game.IsActive
            });
        }

        return PartialView("_GamesListPartial", gamesViewModel);
    }

    public IActionResult News()
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
                CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath!),
                GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                GamePlatform = game.GamePlatform,
                LanguageLevel = game.LanguageLevel,
                PublicationDate = game.PublicationDate,
                SkillsLearning = game.SkillsLearning,
                AverageRaitingPlayers = game.AverageRaitingPlayers,
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

    public async Task<IActionResult> NewsForDevelopers()
    {
        return View();
    }
}
