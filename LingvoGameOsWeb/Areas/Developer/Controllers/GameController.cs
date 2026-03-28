using LingvoGameOs.Areas.Admin.Models;
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
        readonly LocalFileProvider _fileProvider;
        readonly EmailService _emailService;
        readonly GameFileProcessor _gameFileProcessor;
        readonly IFileStorage _fileStorage;

        public GameController(IGamesRepository gamesRepository, UserManager<User> userManager, ILogger<GameController> logger, ISkillsLearningRepository skillsLearningRepository, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IWebHostEnvironment webHostEnvironment, EmailService emailService, GameFileProcessor gameFileProcessor, IFileStorage fileStorage)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _logger = logger;
            _skillsLearningRepository = skillsLearningRepository;
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _fileProvider = new LocalFileProvider(webHostEnvironment);
            _emailService = emailService;
            _gameFileProcessor = gameFileProcessor;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> EditAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileMetadata? msiFileMetadata = null;
            if (existingGame?.GamePlatform?.Name == "Desktop")
            {
                if (existingGame.GameFilePath != null)
                    msiFileMetadata = await _fileStorage.GetFileMetadataAsync(existingGame.GameFilePath);
            }
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View(new EditGameViewModel
            {
                Id = existingGame!.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImagePath = existingGame.CoverImagePath,
                CoverImageMetadata = await _fileStorage.GetFileMetadataAsync(existingGame.CoverImagePath),
                ImagesFilesMetadata = await _gameFileProcessor.GetMetadataListAsync(existingGame.ImagesPaths!),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame.Author,
                AuthorId = existingGame.Author.Id,
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFilePath = existingGame.GameFilePath,
                GameFileMetadata = msiFileMetadata,
                GamePlatform = existingGame?.GamePlatform?.Name!,
                LanguageLevel = existingGame?.LanguageLevel?.Name!,
                VideoUrl = existingGame?.VideoUrl,
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

            if (editGame?.SkillsLearning?[0] == null)
                ModelState.AddModelError("SkillsLearning", "Выберите хотя бы один развиваемый навык");

            if (!ModelState.IsValid)
            {
                var existingGame = await _gamesRepository.TryGetByIdAsync(editGame.Id);

                if (existingGame != null)
                {
                    // Обложка
                    editGame.CoverImageMetadata = await _fileStorage.GetFileMetadataAsync(existingGame.CoverImagePath!);

                    // Скриншоты
                    if (existingGame.ImagesPaths != null && existingGame.ImagesPaths.Any())
                    {
                        editGame.ImagesFilesMetadata = await _gameFileProcessor.GetMetadataListAsync(existingGame.ImagesPaths);
                    }

                    // Файл игры (если Desktop)
                    if (editGame.GamePlatform == "Desktop" && !string.IsNullOrEmpty(existingGame.GameFilePath))
                    {
                        editGame.GameFileMetadata = await _fileStorage.GetFileMetadataAsync(existingGame.GameFilePath);
                        editGame.GameFilePath = existingGame.GameFilePath;
                    }

                    // Автор (для отображения в поле "Автор игры")
                    editGame.Author = existingGame.Author;
                }

                // 2. Снова заполняем список скиллов для Dropdown
                var allSkills = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = allSkills.Select(sl => sl.Name);

                return View(editGame);
            }

            try
            {
                var existingGame = await _gamesRepository.TryGetByIdAsync(editGame.Id);
                if (existingGame == null) return NotFound($"Игра с Id: {editGame.Id} не найдена :(");

                List<string>? selectedSkills = editGame?.SkillsLearning?[0].Split(',').ToList();
                List<SkillLearning> skills = await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                var platform = await _platformsRepository.GetExistingPlatformAsync(editGame.GamePlatform);
                var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(editGame.LanguageLevel);

                existingGame.Title = editGame.Title.Trim();
                existingGame.Description = editGame.Description.Trim();
                existingGame.Rules = editGame.Rules.Trim();
                existingGame.SkillsLearning = skills;
                existingGame.GamePlatform = platform!;
                existingGame.LanguageLevel = languageLvl!;
                existingGame.VideoUrl = editGame.VideoUrl;
                existingGame.LastUpdateDate = DateTimeOffset.UtcNow;
                existingGame.GameGitHubUrl = editGame.GameGitHubUrl;

                // Если есть новое изображение - меняем
                await _gameFileProcessor.ProcessChangeCoverImageAsync(editGame, existingGame, Folders.Games);

                // Процесс удаления картинок
                await _gameFileProcessor.ProcessDeletedImagesAsync(editGame, existingGame);

                // Обрабатываем новые картинки
                await _gameFileProcessor.ProcessUploadNewImagesAsync(editGame, existingGame, Folders.Games);

                // Удаляем файл игры
                await _gameFileProcessor.ProcessDeleteGameFileAsync(editGame, existingGame);

                // Загружаем новый файл Desktop игры, если он есть
                await _gameFileProcessor.ProcessUploadGameFileAsync(editGame, existingGame, Folders.Games);

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
    }
}
