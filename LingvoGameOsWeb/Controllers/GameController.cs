using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class GameController : Controller
    {
        IConfiguration _configuration;
        readonly IGamesRepository _gamesRepository;
        readonly UserManager<User> _userManager;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly FileProvider _fileProvider;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly ILogger<GameController> _logger;
        readonly IFavoriteGamesRepository _favoriteGamesRepository;

        public GameController(
            IGamesRepository gamesRepository,
            UserManager<User> userManager,
            ISkillsLearningRepository skillsLearningRepository,
            IWebHostEnvironment appEnvironment,
            IPlatformsRepository platformsRepository,
            ILanguageLevelsRepository languageLevelsRepository,
            IPendingGamesRepository pendingGamesRepository,
            ILogger<GameController> logger,
            IConfiguration configuration
,
            IFavoriteGamesRepository favoriteGamesRepository)
        {
            _configuration = configuration;

            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _pendingGamesRepository = pendingGamesRepository;
            _logger = logger;
            _favoriteGamesRepository = favoriteGamesRepository;
        }

        public async Task<IActionResult> DetailsAsync(int idGame)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(idGame);
            if (existingGame == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var gameViewModel = new GameViewModel
            {
                Id = existingGame.Id,
                Title = existingGame.Title,
                Author = existingGame.Author,
                CoverImagePath = existingGame.CoverImagePath,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                GameFolderName = existingGame.GameFolderName,
                GameFilePath = existingGame.GameFilePath,
                GamePlatform = existingGame.GamePlatform,
                ImagesPaths = existingGame.ImagesPaths,
                VideoUrl = existingGame.VideoUrl,
                LanguageLevel = existingGame.LanguageLevel,
                PublicationDate = existingGame.PublicationDate,
                SkillsLearning = existingGame.SkillsLearning,
                RaitingPlayers = existingGame.RaitingPlayers,
                RaitingTeachers = existingGame.RaitingTeachers,
                IsFavorite = await _favoriteGamesRepository.IsGameInFavoritesAsync(currentUser?.Id ?? "", existingGame.Id)
            };

            return View(gameViewModel);
        }

        public async Task<IActionResult> StartAsync(int idGame)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(idGame);
            if (existingGame == null)
                return NotFound();

            ViewBag.GameId = idGame;
            ViewBag.GameTitle = existingGame.Title;

            if (existingGame.GameFolderName == null)
                existingGame.GameFolderName = "temp";

            string gameFolder =
                _configuration["ASPNETCORE_ENVIRONMENT"] == "Development"
                    ? Constants.YandexCloudGameFolderPath
                    : Constants.TimeWebCloudGameFolderPath;

            string runningScript = Path.Combine(gameFolder, existingGame.GameFolderName, "run.sh");
            var currentUser = await _userManager.GetUserAsync(User);
            // Добавление игры в историю пользователя
            if (currentUser != null)
                await _gamesRepository.AddPlayerToGameHistoryAsync(existingGame, currentUser);
            var logData = new
            {
                GameId = idGame,
                UserId = currentUser?.Id ?? "Anonymous",
                UserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
                RequestTime = DateTimeOffset.UtcNow,
            };

            if (!System.IO.File.Exists(runningScript))
            {
                _logger.LogError(
                    "Ошибка запуска игры. Отсутсвует скрипт запуска игры {@GameStartData}",
                    new
                    {
                        logData.GameId,
                        logData.UserId,
                        logData.UserIP,
                        logData.UserAgent,
                        logData.RequestTime,
                        GameRunningScript = runningScript,
                        ResponseStatusCode = 500,
                    }
                );

                ViewBag.GameUrl = null;
                return View();
            }

            var runningProcess = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = runningScript,
                WorkingDirectory = Path.GetDirectoryName(runningScript),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                Process.Start(runningProcess);

                string? URL =
                    _configuration["ASPNETCORE_ENVIRONMENT"] == "Development"
                        ? _configuration["DEVELOPMENT_URL"]
                        : _configuration["PRODUCTION_URL"];

                ViewBag.GameUrl = $"{URL}:{existingGame.Port}";

                

                _logger.LogInformation(
                    "Запуск игры {@GameStartData}",
                    new
                    {
                        logData.GameId,
                        logData.UserId,
                        logData.UserIP,
                        logData.UserAgent,
                        logData.RequestTime,
                        ResponseStatusCode = 200,
                    }
                );

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка запуска игры {@GameStartData}",
                    new
                    {
                        logData.GameId,
                        logData.UserId,
                        logData.UserIP,
                        logData.UserAgent,
                        logData.RequestTime,
                        ResponseStatusCode = 500,
                    }
                );

                return BadRequest("Error starting game: " + ex.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> UploadAsync()
        {
            var skillLearnings = await _skillsLearningRepository.GetAllAsync();
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAsync([FromForm] AddGameViewModel gameViewModel)
        {
            string? authorId = _userManager.GetUserId(User);
            var logData = new
            {
                UserId = authorId ?? "Anonymous",
                UserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
            };
            try
            {
                //if (gameViewModel.UploadedGame == null && gameViewModel.GameURL == null)
                //{
                //    ModelState.AddModelError("", "Необходимо добавить ссылку на игру, либо загрузить файл игры");
                //    return View(gameViewModel);
                //}

                //if (gameViewModel.UploadedGame != null && gameViewModel.GameURL != null)
                //{
                //    ModelState.AddModelError("", "Можно выбрать только одну платформу для игры");
                //    return View(gameViewModel);
                //}

                if (ModelState.IsValid)
                {
                    // Получаем из БД выбранные скиллы и платформу
                    List<string>? selectedSkills = JsonSerializer.Deserialize<List<string>>(
                        gameViewModel.SkillsLearning
                    );
                    List<SkillLearning> skills =
                        await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                    var platform = await _platformsRepository.GetExistingPlatformAsync(
                        gameViewModel.GamePlatform
                    );
                    var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(
                        gameViewModel.LanguageLevel
                    );

                    if (gameViewModel.UploadedGame != null) // Если файл загруженной игры не пустой, то она десктоп
                    {
                        PendingGame pendingGame = new PendingGame
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId!,
                            DispatchDate = DateTimeOffset.Now,
                            GamePlatform = platform!,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl!,
                            VideoUrl = gameViewModel.VideoUrl,
                        };
                        await _pendingGamesRepository.AddAsync(pendingGame);

                        // Теперь можно использовать ID для сохранения файлов в соотв. директорию
                        string? gameUrl = await _fileProvider.SaveGameFileAsync(
                            gameViewModel.UploadedGame,
                            pendingGame.Id,
                            pendingGame.Title,
                            Folders.PendingGames
                        );
                        string? coverImagePath = await _fileProvider.SaveGameImgFileAsync(
                            gameViewModel.CoverImage,
                            Folders.PendingGames,
                            pendingGame.Id
                        );
                        List<string> imagesPaths = await _fileProvider.SaveImagesFilesAsync(
                            gameViewModel.UploadedImages,
                            Folders.PendingGames,
                            pendingGame.Id
                        );

                        pendingGame.CoverImagePath = coverImagePath ?? "/img/default-img.jpg";
                        pendingGame.ImagesPaths = imagesPaths;

                        // Если нужно обновить URL игры после сохранения файла
                        if (!string.IsNullOrEmpty(gameUrl))
                        {
                            await _pendingGamesRepository.ChangeGameUrlAsync(gameUrl, pendingGame);
                        }

                        // смена роли игрока на разработчика
                        await ChangeRolePlayerToDevAsync();

                        _logger.LogInformation(
                            "Загрузка игры на модерацию {@UploadGameData}",
                            new
                            {
                                logData.UserId,
                                logData.UserIP,
                                logData.UserAgent,
                                RequestTime = DateTimeOffset.UtcNow,
                                PendingGameId = pendingGame.Id,
                                PendingGamePlatform = pendingGame.GamePlatform.Name,
                                ResponseStatusCode = 200,
                            }
                        );
                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return Json(
                            new
                            {
                                success = true,
                                redirectUrl = Url.Action(
                                    "Index",
                                    "Profile",
                                    new { userId = authorId }
                                ),
                            }
                        );
                    }
                    else // Инача - игра веб
                    {
                        PendingGame pendingGame = new PendingGame
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId!,
                            DispatchDate = DateTimeOffset.UtcNow,
                            GamePlatform = platform!,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl!,
                            GameURL = gameViewModel.GameURL!,
                            VideoUrl = gameViewModel.VideoUrl,
                        };
                        await _pendingGamesRepository.AddAsync(pendingGame);

                        string? coverImagePath = await _fileProvider.SaveGameImgFileAsync(
                            gameViewModel.CoverImage,
                            Folders.PendingGames,
                            pendingGame.Id
                        );
                        List<string> imagesPaths = await _fileProvider.SaveImagesFilesAsync(
                            gameViewModel.UploadedImages,
                            Folders.PendingGames,
                            pendingGame.Id
                        );

                        await _pendingGamesRepository.ChangeImagesAsync(
                            coverImagePath ?? "/img/default-img.jpg",
                            imagesPaths,
                            pendingGame
                        );

                        // смена роли игрока на разработчика
                        await ChangeRolePlayerToDevAsync();

                        _logger.LogInformation(
                            "Загрузка игры на модерацию {@UploadGameData}",
                            new
                            {
                                logData.UserId,
                                logData.UserIP,
                                logData.UserAgent,
                                RequestTime = DateTimeOffset.UtcNow,
                                PendingGameId = pendingGame.Id,
                                PendingGamePlatform = pendingGame.GamePlatform.Name,
                                ResponseStatusCode = 200,
                            }
                        );
                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return Json(
                            new
                            {
                                success = true,
                                redirectUrl = Url.Action(
                                    "Index",
                                    "Profile",
                                    new { userId = authorId }
                                ),
                            }
                        );
                    }
                }

                // Нужно чтобы отобразить список скиллов из БД для игры
                var skillLearnings = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
                return View(gameViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка загрузки игры на модерацию {@UploadGameData}",
                    new
                    {
                        logData.UserId,
                        logData.UserIP,
                        logData.UserAgent,
                        RequestTime = DateTimeOffset.UtcNow,
                        ResponseStatusCode = 500,
                    }
                );
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // смена роли игрока на разработчика
        private async Task ChangeRolePlayerToDevAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // если есть роль игрока, то удаляю ее
                if (await _userManager.IsInRoleAsync(currentUser, Constants.PlayerRoleName))
                {
                    await _userManager.RemoveFromRoleAsync(currentUser, Constants.PlayerRoleName);
                }
                // если нет роли разработчика, то добавляю ее
                if (!await _userManager.IsInRoleAsync(currentUser, Constants.DevRoleName))
                {
                    await _userManager.AddToRoleAsync(currentUser, Constants.DevRoleName);
                }
            }
        }
    }
}
