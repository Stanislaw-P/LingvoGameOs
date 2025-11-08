using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Areas.Developers.Controllers
{
    [Area("Developer")]
    [Authorize(Roles = Constants.DevRoleName)]
    public class GameController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly UserManager<User> _userManager;
        readonly ILogger<GameController> _logger;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly FileProvider _fileProvider;
        readonly EmailService _emailService;

        public GameController(IGamesRepository gamesRepository, UserManager<User> userManager, ILogger<GameController> logger, ISkillsLearningRepository skillsLearningRepository, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IWebHostEnvironment webHostEnvironment, EmailService emailService)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _logger = logger;
            _skillsLearningRepository = skillsLearningRepository;
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _fileProvider = new FileProvider(webHostEnvironment);
            _emailService = emailService;
        }

        public async Task<IActionResult> EditAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
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
            return View(new EditGameViewModel
            {
                Id = existingGame.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImagePath = existingGame.CoverImagePath,
                CoverImageInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.CoverImagePath)),
                ImagesFilesInfo = _fileProvider.GetImagesFilesInfo(existingGame.ImagesPaths),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame.Author,
                AuthorId = existingGame.Author.Id,
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFilePath = existingGame.GameFilePath,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl,
                LastUpdateDate = existingGame.LastUpdateDate,
                PublicationDate = existingGame.PublicationDate,
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditGameViewModel editGame)
        {
            var devUserId = _userManager.GetUserId(User);
            var logData = new
            {
                GameId = editGame.Id,
                DeveloperUserId = devUserId,
                DeveloperUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeveloperUserAgent = Request.Headers.UserAgent.ToString(),
            };
            
            // Нужно чтобы отобразить список скиллов из БД для игры
            var skillLearnings = await _skillsLearningRepository.GetAllAsync();
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);          

            if (!ModelState.IsValid)
                return View(editGame);

            try
            {
                var existingGame = await _gamesRepository.TryGetByIdAsync(editGame.Id);
                if (existingGame == null) return NotFound($"Игра с Id: {editGame.Id} не найдена :(");

                List<string>? selectedSkills = editGame?.SkillsLearning?[0].Split(',').ToList();
                List<SkillLearning> skills = await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                var platform = await _platformsRepository.GetExistingPlatformAsync(editGame.GamePlatform);
                var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(editGame.LanguageLevel);

                existingGame.Description = editGame.Description.Trim();
                existingGame.Rules = editGame.Rules.Trim();
                existingGame.SkillsLearning = skills;
                existingGame.GamePlatform = platform!;
                existingGame.LanguageLevel = languageLvl!;
                existingGame.VideoUrl = editGame.VideoUrl;
                existingGame.LastUpdateDate = DateTimeOffset.UtcNow;
                existingGame.GameGitHubUrl = editGame.GameGitHubUrl;
                if (editGame.GamePlatform != "Desktop")
                    existingGame.Title = editGame.Title.Trim();

                // Если есть новое изображение - меняем
                await ProcessChangeCoverImageAsync(editGame, existingGame);

                // Процесс удаления картинок
                ProcessDeletedImages(editGame, existingGame);

                // Обрабатываем новые картинки
                await ProcessNewImagesAsync(editGame, existingGame);

                // Удаляем файл игры
                ProcessDeleteGameFile(editGame, existingGame);

                // Меняем имя файла Desktop игры
                ProcessRenameGameFile(editGame, existingGame);

                // Меняем путь к файлу игры, если он изменился
                ProcessChangeGameFilePath(editGame, existingGame);

                // Деактивируем игру, дабы админ подтвердил изменения
                existingGame.IsActive = false;

                await _gamesRepository.UpdateAsync(existingGame);

                await _emailService.TrySendGameUpdateByDevAsync(existingGame.Author.Name, existingGame.Author.Email!, existingGame.Title, existingGame.Id);

                _logger.LogInformation("Именение данных игры разработчиком {@GameEditData}", new
                {
                    logData.GameId,
                    logData.DeveloperUserId,
                    logData.DeveloperUserIP,
                    logData.DeveloperUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 200
                });

                return Redirect($"/Profile/Index?userId={devUserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка изменения игры разработчиком {@GameEditData}", new
                {
                    logData.GameId,
                    logData.DeveloperUserId,
                    logData.DeveloperUserIP,
                    logData.DeveloperUserAgent,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 500
                });
                return BadRequest(ex.Message);
            }
        }

        private void ProcessChangeGameFilePath(EditGameViewModel editGame, Game existingGame)
        {
            if (editGame.GameFilePath != editGame.CurrentGameFilePath)
            {
                existingGame.GameFilePath = editGame.GameFilePath;
            }
        }

        private async Task ProcessChangeCoverImageAsync(EditGameViewModel editGame, Game existingGame)
        {
            if (editGame.CoverImage != null)
            {
                string? coverImagePath = await _fileProvider.SaveGameImgFileAsync(editGame.CoverImage, Folders.Games, existingGame.Id);
                existingGame.CoverImagePath = coverImagePath;

                // Удаляем прошлую обложку
                _fileProvider.DeleteFile(editGame.CurrentCoverImagePath!);
            }
            else
                existingGame.CoverImagePath = editGame.CurrentCoverImagePath;
        }

        private void ProcessDeletedImages(EditGameViewModel editGameViewModel, Game game)
        {
            if (editGameViewModel.DeletedImages == null || !editGameViewModel.DeletedImages.Any())
                return;

            // Удаляем файлы из системы
            _fileProvider.DeleteImages(editGameViewModel.DeletedImages, Folders.Games, game.Id);

            //Удаляем пути из БД
            if (game.ImagesPaths != null)
            {
                var deletedImagesPaths = new List<string>();
                foreach (var deletedImg in editGameViewModel.DeletedImages)
                {
                    var deletedImagePath = _fileProvider.GetGameFileShortPath(deletedImg, Folders.Games, game.Id);
                    deletedImagesPaths.Add(deletedImagePath);
                }
                game.ImagesPaths = game.ImagesPaths
                    .Where(imgPath => !deletedImagesPaths.Contains(imgPath))
                    .ToList();
            }
        }

        private async Task ProcessNewImagesAsync(EditGameViewModel editGameViewModel, Game game)
        {
            if (editGameViewModel.UploadedImages == null || !editGameViewModel.UploadedImages.Any(f => f.Length > 0))
                return;

            List<string> newImagesPaths = await _fileProvider.SaveImagesFilesAsync(
                editGameViewModel.UploadedImages.Where(f => f.Length > 0).ToArray(),
                Folders.Games, editGameViewModel.Id);

            if (game.ImagesPaths == null)
                game.ImagesPaths = new List<string>();
            game.ImagesPaths.AddRange(newImagesPaths);
        }

        private void ProcessDeleteGameFile(EditGameViewModel editGame, Game existingGame)
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

        private void ProcessRenameGameFile(EditGameViewModel editGame, Game existingGame)
        {
            if (editGame.GamePlatform == "Desktop")
            {
                if (editGame.Title != existingGame.Title && existingGame.GameFilePath != null)
                {
                    string newGameFileName = editGame.Title.Trim();
                    // TODO: Необходимо будет изменить код если появятся Unity игры
                    string newGameFilePath = _fileProvider.UpdateFileName(editGame.CurrentGameFilePath!, newGameFileName + ".msi");
                    editGame.GameFilePath = newGameFilePath;
                    existingGame.Title = newGameFileName;
                    _fileProvider.MoveGameFile(existingGame.GameFilePath, editGame.GameFilePath);
                }
            }
        }
    }
}
