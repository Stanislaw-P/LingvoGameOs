using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class PendingGamesController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly IGamesRepository _gamesRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly FileProvider _fileProvider;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly ILogger<PendingGamesController> _logger;
        readonly EmailService _emailService;
        readonly S3Service _s3Service;
        readonly GameFileProcessor _gameFileProcessor;

        public PendingGamesController(UserManager<User> userManager, IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IGamesRepository gamesRepository, ILogger<PendingGamesController> logger, EmailService emailService, S3Service s3Service, GameFileProcessor gameFileProcessor)
        {
            _userManager = userManager;
            _pendingGamesRepository = pendingGamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _gamesRepository = gamesRepository;
            _logger = logger;
            _emailService = emailService;
            _s3Service = s3Service;
            _gameFileProcessor = gameFileProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> PublishAsync(int gameId)
        {
            PendingGame? pendingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (pendingGame == null)
                return NotFound();

            var adminUserId = _userManager.GetUserId(User);
            var logData = new
            {
                PendingGameId = gameId,
                AdminUserId = adminUserId,
                AdminUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                AdminUserAgent = Request.Headers.UserAgent.ToString(),
            };

            try
            {
                var publishedGame = await _pendingGamesRepository.PublishAsync(pendingGame);

                await _gameFileProcessor.PublishGameFilesAsync(pendingGame, publishedGame);

                await _gamesRepository.UpdateAsync(publishedGame);

                _logger.LogInformation("Успешная публикация игры {@PendingGamePublishData}", new
                {
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    PendingGameId = pendingGame.Id,
                    PendingGamePlatform = pendingGame?.GamePlatform?.Name,
                    NewGameId = publishedGame.Id,
                    ResponseStatusCode = 200
                });

                await _emailService.TrySendAboutPublicationAsync(publishedGame.Author.Name, publishedGame?.Author?.Email!, publishedGame?.Title!);

                return Ok(new
                {
                    success = true,
                    gameData = new
                    {
                        id = publishedGame?.Id,
                        title = publishedGame?.Title,
                        authorName = $"{publishedGame?.Author.Name} {publishedGame?.Author.Surname}",
                        publicationDate = publishedGame?.PublicationDate.ToString("dd.MM.yyyy"),
                        imagePath = _s3Service.GetPublicUrl(publishedGame?.CoverImagePath!),
                        gameUrl = publishedGame?.GameFilePath,
                        platform = publishedGame?.GamePlatform?.Name
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка публикации игры {@PendingGamePublishData}", new
                {
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 500
                });
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int gameId)
        {
            // 1. Получаем объект черновика из репозитория
            var pendingGameForDelete = await _pendingGamesRepository.TryGetByIdAsync(gameId);

            if (pendingGameForDelete == null)
                return NotFound();

            // 2. Удаляем все файлы, связанные с этим черновиком, из S3
            // Формируем путь вида: "PendingGames/123"
            string s3FolderPrefix = $"{Folders.PendingGames}/{gameId}";
            await _s3Service.DeleteDirectoryAsync(s3FolderPrefix);

            // 3. Удаляем запись из базы данных
            await _pendingGamesRepository.RemoveAsync(pendingGameForDelete);

            // 4. Отправляем уведомление автору об отказе/удалении
            if (pendingGameForDelete.Author != null)
            {
                await _emailService.TrySendRefusalGameAsync(
                    pendingGameForDelete.Author.Name ?? "Разработчик",
                    pendingGameForDelete.Author.Email ?? "",
                    pendingGameForDelete.Title ?? "Без названия");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendFeedbackAsync([FromBody] FeedBackViewModel feedBackView)
        {
            if (feedBackView.Message == null)
            {
                ModelState.AddModelError("Message", "Сообщение не может быть пустым!");
                return BadRequest(new { error = "Сообщение не может быть пустым" });
            }
            if (feedBackView.GameId < 0)
            {
                ModelState.AddModelError("GameId", "ID игры не может быть отрицательным!");
                return BadRequest(new { error = "ID игры не может быть отрицательным!" });
            }
            var adminUserId = _userManager.GetUserId(User);
            var logData = new
            {
                PendingGameId = feedBackView.GameId,
                AdminUserId = adminUserId,
                AdminUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                AdminUserAgent = Request.Headers.UserAgent.ToString(),
            };
            try
            {
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(feedBackView.GameId);
                if (existingGame == null)
                    return NotFound(new { error = $"Игра с id: {feedBackView.GameId} не найдена!" });
                existingGame.LastMessage = feedBackView.Message;

                if (!string.IsNullOrEmpty(existingGame?.Author?.Email))
                    await _emailService.TrySendModerationRejectionAsync(existingGame?.Author?.Name ?? "Разработчик", existingGame?.Author.Email!, existingGame?.Title!, feedBackView.Message); ;

                await _pendingGamesRepository.UpdateAsync(existingGame);

                _logger.LogInformation("Сообщение разработчику отправлено {@FeedbackPendingGameData}", new
                {
                    logData.PendingGameId,
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    DevUserId = existingGame.Author.Id,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 200
                });
                return Ok(new
                {
                    success = true,
                    message = "Сообщение разработчику отправлено"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки сообщения разработчику {@FeedbackPendingGameData}", new
                {
                    logData.PendingGameId,
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 500
                });
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public async Task<IActionResult> GetGameInfoAsync(int gameId)
        {
            try
            {
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
                if (existingGame == null)
                    return NotFound(new { error = $"Игра с id: {gameId} не найдена" });
                return Ok(new
                {
                    title = existingGame.Title,
                    author = $"{existingGame?.Author?.Name} {existingGame?.Author?.Surname}",
                    authorEmail = existingGame?.Author?.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }

        public async Task<IActionResult> DetailsAsync(int gameId)
        {
            var existingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileMetadata? msiFileMetadata = null;
            if (existingGame?.GamePlatform?.Name == "Desktop")
            {
                if (existingGame.GameFilePath != null)
                    msiFileMetadata = await _s3Service.GetFileMetadataAsync(existingGame.GameFilePath);
            }
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View(new AdminEditGameViewModel
            {
                Id = existingGame!.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImagePath = existingGame.CoverImagePath,
                CoverImageMetadata = await _s3Service.GetFileMetadataAsync(existingGame.CoverImagePath),
                ImagesFilesMetadata = await _s3Service.GetFilesMetadataAsync(existingGame.ImagesPaths!),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame?.Author,
                AuthorId = existingGame?.Author?.Id!,
                DispatchDate = existingGame.DispatchDate,
                GameFilePath = _s3Service.GetPublicUrl(existingGame.GameFilePath!),
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFolderName = existingGame.GameFolderName,
                GameFileMetadata = msiFileMetadata,
                GamePlatform = existingGame?.GamePlatform?.Name!,
                LanguageLevel = existingGame?.LanguageLevel?.Name!,
                VideoUrl = existingGame?.VideoUrl,
                LastMessage = existingGame?.LastMessage,
                Port = existingGame!.Port
            });
        }

        [HttpPost]
        public async Task<IActionResult> DetailsAsync(AdminEditGameViewModel editGame)
        {
            if (!ModelState.IsValid)
            {
                var skillLearnings = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);

                var authorGame = await _userManager.FindByIdAsync(editGame.AuthorId);
                if (authorGame != null)
                    editGame.Author = authorGame;

                return View(editGame);
            }

            var adminUserId = _userManager.GetUserId(User);
            var logData = new
            {
                PendingGameId = editGame.Id,
                AdminUserId = adminUserId,
                AdminUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                AdminUserAgent = Request.Headers.UserAgent.ToString(),
            };

            try
            {
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(editGame.Id);
                if (existingGame == null) return NotFound($"Игра с Id: {editGame.Id} не найдена :(");

                if (editGame.SkillsLearning == null || editGame.SkillsLearning.Count == 0)
                {
                    ModelState.AddModelError("SkillsLearning", "Нужно выбрать хотя бы один развиваемый навык!");
                    return View(editGame);
                }

                // Получаем данные с формы
                List<string>? selectedSkills = editGame.SkillsLearning[0].Split(',').ToList();
                List<SkillLearning> skills = await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                var platform = await _platformsRepository.GetExistingPlatformAsync(editGame.GamePlatform);
                var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(editGame.LanguageLevel);

                existingGame.Title = editGame.Title.Trim();
                existingGame.Description = editGame.Description;
                existingGame.Rules = editGame.Rules;
                existingGame.SkillsLearning = skills;
                existingGame.GamePlatform = platform!;
                existingGame.LanguageLevel = languageLvl!;
                existingGame.VideoUrl = editGame.VideoUrl;
                existingGame.GameFolderName = editGame.GameFolderName;
                existingGame.LastUpdateDate = DateTimeOffset.UtcNow;
                existingGame.GameGitHubUrl = editGame.GameGitHubUrl;
                existingGame.Port = editGame.Port;


                // Если есть новое изображение - меняем
                await _gameFileProcessor.ProcessChangeCoverImageAsync(editGame, existingGame, Folders.PendingGames);

                // Процесс удаления картинок
                await _gameFileProcessor.ProcessDeletedImagesAsync(editGame, existingGame);

                // Обрабатываем новые картинки
                await _gameFileProcessor.ProcessNewImagesAsync(editGame, existingGame, Folders.PendingGames);

                // Удаляем файл игры
                await _gameFileProcessor.ProcessDeleteGameFileAsync(editGame, existingGame);

                await _pendingGamesRepository.UpdateAsync(existingGame);

                _logger.LogInformation("Изменение данных игры на модерации {@PendingGameEditData}", new
                {
                    logData.PendingGameId,
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    DevUserId = existingGame.Author.Id,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 200
                });
                return Redirect("/Admin/Home/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка изменения игры на модерации {@PendingGameEditData}", new
                {
                    logData.PendingGameId,
                    logData.AdminUserId,
                    logData.AdminUserIP,
                    logData.AdminUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 500
                });
                return BadRequest(ex.Message);
            }
        }
    }
}
