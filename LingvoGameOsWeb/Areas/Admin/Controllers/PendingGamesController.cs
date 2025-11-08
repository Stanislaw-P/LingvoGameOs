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

        public PendingGamesController(UserManager<User> userManager, IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IGamesRepository gamesRepository, ILogger<PendingGamesController> logger, EmailService emailService)
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
                // Переносим файлы
                _fileProvider.MoveGameFiles(
                    pendingGame.Id,
                    publishedGame.Id,
                    Folders.PendingGames,
                    Folders.Games);

                // Обновляем пути к файлам
                publishedGame.CoverImagePath = _fileProvider.UpdateFilePath(
                    pendingGame.CoverImagePath!,
                    $"{Folders.PendingGames}/{pendingGame.Id}",
                    $"{Folders.Games}/{publishedGame.Id}");

                if (publishedGame?.GamePlatform?.Name == "Desktop")
                {
                    publishedGame.GameFilePath = _fileProvider.UpdateFilePath(
                    pendingGame.GameFilePath!,
                    $"{Folders.PendingGames}/{pendingGame.Id}",
                    $"{Folders.Games}/{publishedGame.Id}");
                }

                publishedGame.ImagesPaths = new List<string>();
                // Переносим скриншоты
                foreach (var imgPath in pendingGame.ImagesPaths!)
                {
                    var newImgPath = _fileProvider.UpdateFilePath(
                        imgPath,
                        $"{Folders.PendingGames}/{pendingGame.Id}",
                        $"{Folders.Games}/{publishedGame.Id}");

                    publishedGame.ImagesPaths.Add(newImgPath);
                }

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
                        imagePath = publishedGame?.CoverImagePath,
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
            var pendingGameForDelete = await _pendingGamesRepository.TryGetByIdAsync(gameId);

            if (pendingGameForDelete == null)
                return NotFound();

            string directoryPendingGamePath = _fileProvider.GetGameDirectoryPath(gameId, Folders.PendingGames);
            _fileProvider.DeleteDirectory(directoryPendingGamePath);

            await _pendingGamesRepository.RemoveAsync(pendingGameForDelete);
            await _emailService.TrySendRefusalGameAsync(pendingGameForDelete?.Author?.Name ?? "Разработчик", pendingGameForDelete?.Author?.Email!, pendingGameForDelete?.Title!);
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
                    author = $"{existingGame.Author.Name} {existingGame.Author.Surname}",
                    authorEmail = existingGame.Author.Email
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

            FileInfo? msiFileInfo = null;
            if (existingGame?.GamePlatform?.Name == "Desktop")
            {
                if (existingGame.GameFilePath != null)
                    msiFileInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.GameFilePath));
            }
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View(new AdminEditGameViewModel
            {
                Id = existingGame.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImagePath = existingGame.CoverImagePath,
                CoverImageInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.CoverImagePath)),
                ImagesFilesInfo = _fileProvider.GetImagesFilesInfo(existingGame.ImagesPaths),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame?.Author,
                AuthorId = existingGame?.Author?.Id,
                DispatchDate = existingGame.DispatchDate,
                GameFilePath = existingGame.GameFilePath,
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFolderName = existingGame.GameFolderName,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl,
                LastMessage = existingGame.LastMessage,
                Port = existingGame.Port
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
                await ProcessChangeCoverImageAsync(editGame, existingGame);

                // Процесс удаления картинок
                ProcessDeletedImages(editGame, existingGame);

                // Обрабатываем новые картинки
                await ProcessNewImagesAsync(editGame, existingGame);

                // Удаляем файл игры
                ProcessDeleteGameFile(editGame, existingGame);

                // Меняем ия
                ProcessRenameGameFile(editGame, existingGame);

                // Меняем путь к файлу игры, если он изменился
                ProcessChangeGameFilePath(editGame, existingGame);

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

        private void ProcessChangeGameFilePath(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GameFilePath != editGame.CurrentGameFilePath)
            {
                existingGame.GameFilePath = editGame.GameFilePath;
            }
        }

        private async Task ProcessChangeCoverImageAsync(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.CoverImage != null)
            {
                string? coverImagePath = await _fileProvider.SaveGameImgFileAsync(editGame.CoverImage, Folders.PendingGames, existingGame.Id);
                existingGame.CoverImagePath = coverImagePath;

                // Удаляем прошлую обложку
                _fileProvider.DeleteFile(editGame.CurrentCoverImagePath!);
            }
            else
                existingGame.CoverImagePath = editGame.CurrentCoverImagePath;
        }

        private void ProcessDeletedImages(EditGameViewModel editGameViewModel, PendingGame pendingGame)
        {
            if (editGameViewModel.DeletedImages == null || !editGameViewModel.DeletedImages.Any())
                return;

            // Удаляем файлы из системы
            _fileProvider.DeleteImages(editGameViewModel.DeletedImages, Folders.PendingGames, pendingGame.Id);

            //Удаляем пути из БД
            if (pendingGame.ImagesPaths != null)
            {
                var deletedImagesPaths = new List<string>();
                foreach (var deletedImg in editGameViewModel.DeletedImages)
                {
                    var deletedImagePath = _fileProvider.GetGameFileShortPath(deletedImg, Folders.PendingGames, pendingGame.Id);
                    deletedImagesPaths.Add(deletedImagePath);
                }
                pendingGame.ImagesPaths = pendingGame.ImagesPaths
                    .Where(imgPath => !deletedImagesPaths.Contains(imgPath))
                    .ToList();
            }
        }

        private async Task ProcessNewImagesAsync(EditGameViewModel editGameViewModel, PendingGame pendingGame)
        {
            if (editGameViewModel.UploadedImages == null || !editGameViewModel.UploadedImages.Any(f => f.Length > 0))
                return;

            List<string> newImagesPaths = await _fileProvider.SaveImagesFilesAsync(
                editGameViewModel.UploadedImages.Where(f => f.Length > 0).ToArray(),
                Folders.PendingGames, editGameViewModel.Id);

            if (pendingGame.ImagesPaths == null)
                pendingGame.ImagesPaths = new List<string>();
            pendingGame.ImagesPaths.AddRange(newImagesPaths);
        }

        private void ProcessDeleteGameFile(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GamePlatform == "Desktop" && editGame.GameFilePath == null)
            {
                if (editGame.CurrentGameFilePath != null)
                {
                    existingGame.GameFilePath = null;
                    _fileProvider.DeleteFile(editGame.CurrentGameFilePath);
                }
            }
        }

        private void ProcessRenameGameFile(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GamePlatform == "Desktop")
            {
                if (editGame.Title != existingGame.Title && existingGame.GameFilePath != null)
                {
                    string newGameFileName = editGame.Title.Trim();
                    string newGameFilePath = _fileProvider.UpdateFileName(editGame.CurrentGameFilePath!, newGameFileName + ".msi");
                    editGame.GameFilePath = newGameFilePath;
                    existingGame.Title = newGameFileName;
                    _fileProvider.MoveGameFile(existingGame.GameFilePath, editGame.GameFilePath);
                }
                else
                    existingGame.Title = editGame.Title.Trim();
            }
            else
                existingGame.Title = editGame.Title.Trim();
        }
    }
}
