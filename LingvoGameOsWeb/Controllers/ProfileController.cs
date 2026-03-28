using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Helpers;

namespace LingvoGameOs.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        readonly IGamesRepository gamesRepository;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly IFavoriteGamesRepository _favoriteGamesRepository;
        readonly IFileStorage _fileStorage;
        readonly ILogger<ProfileController> _logger;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository, IFavoriteGamesRepository favoriteGamesRepository, IFileStorage fileStorage, ILogger<ProfileController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
            _favoriteGamesRepository = favoriteGamesRepository;
            _fileStorage = fileStorage;
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var devGames = await gamesRepository.TryGetUserDevGamesAsync(user);
                var devGamesViewModel = new List<GameViewModel>();

                foreach (var game in devGames)
                {
                    var gameViewModel = new GameViewModel
                    {
                        // TODO: Думаю можно упростить данный код, так как большинство полей не используется в отображении!
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
                        IsActive = game.IsActive,
                    };
                    devGamesViewModel.Add(gameViewModel);
                }

                var pendingGames = await _pendingGamesRepository.TryGetUserDevGamesAsync(user);
                var devPendingGamesViewModel = new List<PendingGameViewModel>();

                foreach (var pendingGame in pendingGames)
                {
                    var pendingGameViewModel = new PendingGameViewModel
                    {
                        Id = pendingGame.Id,
                        Title = pendingGame.Title,
                        Author = pendingGame.Author,
                        CoverImagePath = _fileStorage.GetPublicUrl(pendingGame.CoverImagePath!),
                        GameFilePath = _fileStorage.GetDownloadUrl(pendingGame.GameFilePath!, pendingGame.Title, ".msi"),
                        GamePlatform = pendingGame.GamePlatform
                    };
                    devPendingGamesViewModel.Add(pendingGameViewModel);
                }

                var gamesHistory = await gamesRepository.TryGetUserGameHistoryAsync(user);
                var gamesHistoryViewModel = new List<GameViewModel>();
                foreach (var game in gamesHistory)
                {
                    var gameHistory = new GameViewModel
                    {
                        Id = game.Id,
                        Title = game.Title,
                        Description = game.Description,
                        CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath!)
                    };
                    gamesHistoryViewModel.Add(gameHistory);
                }

                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName ?? "Пользователь",
                    Description = user.Description,
                    AvatarImgPath = _fileStorage.GetPublicUrl(user.AvatarImgPath),
                    DevGames = devGamesViewModel,
                    DevPendingGames = devPendingGamesViewModel,
                    GamesHistory = gamesHistoryViewModel,
                    TotalPoints = user.TotalPoints
                };

                User? UserProfileOwner = await userManager.GetUserAsync(User);
                if (userId == UserProfileOwner?.Id)
                {
                    userViewModel.IsMyProfile = true;
                }

                return View(userViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SettingsAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                User? UserProfileOwner = await userManager.GetUserAsync(User);
                if (userId == UserProfileOwner?.Id)
                {
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        Surname = user.Surname,
                        Description = user.Description,
                        AvatarImgPath = _fileStorage.GetPublicUrl(user.AvatarImgPath!)
                    });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SettingsAsync(EditUserViewModel settings)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                string modelUserName = settings.UserName.Trim();
                if (user.UserName != modelUserName)
                {
                    var newEmailUser = await userManager.FindByEmailAsync(modelUserName);
                    if (newEmailUser == null)
                    {
                        var token = await userManager.GenerateChangeEmailTokenAsync(user, modelUserName);
                        var result = await userManager.ChangeEmailAsync(user, modelUserName, token);
                        user.UserName = modelUserName;
                        user.NormalizedUserName = userManager.NormalizeName(modelUserName);
                        user.EmailConfirmed = false;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Такой email уже используется");
                    }
                }
                if (user.Name != settings.Name)
                {
                    user.Name = settings.Name.Trim();
                }
                if (user.Surname != settings.Surname)
                {
                    user.Surname = settings.Surname.Trim();
                }
                if (user.Description != settings.Description)
                {
                    user.Description = settings?.Description?.Trim();
                }
                if (settings?.UploadedFile != null)
                {
                    if (settings.UploadedFile.ContentType.Split("/")[0] == "image")
                    {
                        try
                        {
                            string oldAvatarPath = user.AvatarImgPath;
                            user.AvatarImgPath = await _fileStorage.UploadAvatarFileAsync(settings.UploadedFile, Folders.Avatars);

                            if (settings.AvatarImgPath != null && !settings.AvatarImgPath.EndsWith("Avatars/avatar100.png") && oldAvatarPath != user.AvatarImgPath)
                            {
                                await _fileStorage.DeleteFileAsync(oldAvatarPath);
                            }
                            settings.AvatarImgPath = user.AvatarImgPath;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Ошибка изменения аватарки {@AvatarEditData}", new
                            {
                                user.Id,
                                RequestTime = DateTimeOffset.UtcNow,
                                ResponseStatusCode = 500
                            });
                            return View(settings);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Загрузите файл изображения");
                    }
                }
                await userManager.UpdateAsync(user);
                await signInManager.RefreshSignInAsync(user);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Profile", new { userId = user.Id });
            }
            return View(settings);
        }
    }
}
