using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Areas.Developer.Controllers
{
    [Area("Developer")]
    [Authorize(Roles = Constants.DevRoleName)]
    public class PendingGameController : Controller
    {
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly FileProvider _fileProvider;
        readonly EmailService _emailService;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILogger<PendingGameController> _logger;
        readonly UserManager<User> _userManager;
        readonly S3Service _s3Service;
        readonly GameFileProcessor _gameFileProcessor;

        public PendingGameController(IPendingGamesRepository pendingGamesRepository, IWebHostEnvironment appEnvironment, EmailService emailService, ISkillsLearningRepository skillsLearningRepository, ILanguageLevelsRepository languageLevelsRepository, IPlatformsRepository platformsRepository, ILogger<PendingGameController> logger, UserManager<User> userManager, S3Service s3Service, GameFileProcessor gameFileProcessor)
        {
            _pendingGamesRepository = pendingGamesRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _emailService = emailService;
            _skillsLearningRepository = skillsLearningRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _platformsRepository = platformsRepository;
            _logger = logger;
            _userManager = userManager;
            _s3Service = s3Service;
            _gameFileProcessor = gameFileProcessor;
        }

        public async Task<IActionResult> EditAsync(int pendingGameId)
        {
            var existingGame = await _pendingGamesRepository.TryGetByIdAsync(pendingGameId);
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
            return View(new EditGameViewModel
            {
                Id = existingGame.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImagePath = existingGame.CoverImagePath,
                CoverImageMetadata = await _s3Service.GetFileMetadataAsync(existingGame.CoverImagePath!),
                ImagesFilesMetadata = await _s3Service.GetFilesMetadataAsync(existingGame.ImagesPaths!),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame?.Author,
                AuthorId = existingGame?.Author?.Id!,
                DispatchDate = existingGame.DispatchDate,
                GameFilePath = _s3Service.GetPublicUrl(existingGame.GameFilePath!),
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFileMetadata = msiFileMetadata,
                GamePlatform = existingGame?.GamePlatform?.Name!,
                LanguageLevel = existingGame?.LanguageLevel?.Name!,
                VideoUrl = existingGame?.VideoUrl,
                LastMessage = existingGame?.LastMessage,
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditGameViewModel editGame)
        {
            if (editGame?.SkillsLearning?[0] == null)
                ModelState.AddModelError("SkillsLearning", "Выберите хотя бы один развиваемый навык");

            if (!ModelState.IsValid)
            {
                // Получаем оригинал из репозитория, чтобы восстановить метаданные S3
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(editGame.Id);

                if (existingGame != null)
                {
                    // Восстанавливаем автора
                    editGame.Author = await _userManager.FindByIdAsync(editGame.AuthorId) ?? existingGame.Author;

                    // Восстанавливаем данные обложки из S3
                    if (!string.IsNullOrEmpty(existingGame.CoverImagePath))
                    {
                        editGame.CurrentCoverImagePath = existingGame.CoverImagePath;
                        editGame.CoverImageMetadata = await _s3Service.GetFileMetadataAsync(existingGame.CoverImagePath);
                    }

                    // Восстанавливаем данные скриншотов из S3
                    if (existingGame.ImagesPaths != null && existingGame.ImagesPaths.Any())
                    {
                        editGame.ImagesFilesMetadata = await _s3Service.GetFilesMetadataAsync(existingGame.ImagesPaths);
                    }

                    // Восстанавливаем данные файла игры (если Desktop)
                    if (editGame.GamePlatform == "Desktop" && !string.IsNullOrEmpty(existingGame.GameFilePath))
                    {
                        // Заполняем метаданные для отображения имени/размера в UI
                        editGame.GameFileMetadata = await _s3Service.GetFileMetadataAsync(existingGame.GameFilePath);
                        // Публичная ссылка для скачивания/проверки
                        editGame.GameFilePath = existingGame.GameFilePath;
                    }
                }

                // Перезаполняем список всех доступных скиллов для Dropdown/Select2
                var allSkills = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = allSkills.Select(sl => sl.Name);

                return View(editGame);
            }


            var devUserId = _userManager.GetUserId(User);
            var logData = new
            {
                PendingGameId = editGame.Id,
                DeveloperUserId = devUserId,
                DeveloperUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeveloperUserAgent = Request.Headers.UserAgent.ToString(),
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
                List<string>? selectedSkills = editGame?.SkillsLearning?[0].Split(',').ToList();
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
                existingGame.LastUpdateDate = DateTimeOffset.UtcNow;
                existingGame.GameGitHubUrl = editGame.GameGitHubUrl;

                // Если есть новое изображение - меняем
                await _gameFileProcessor.ProcessChangeCoverImageAsync(editGame, existingGame, Folders.PendingGames);

                // Процесс удаления картинок
                await _gameFileProcessor.ProcessDeletedImagesAsync(editGame, existingGame);

                // Обрабатываем новые картинки
                await _gameFileProcessor.ProcessUploadNewImagesAsync(editGame, existingGame, Folders.PendingGames);

                // Удаляем файл игры
                await _gameFileProcessor.ProcessDeleteGameFileAsync(editGame, existingGame);

                // Загружаем новый файл Desktop игры, если он есть
                await _gameFileProcessor.ProcessUploadGameFileAsync(editGame, existingGame, Folders.PendingGames);
                
                await _pendingGamesRepository.UpdateAsync(existingGame);

                _logger.LogInformation("Изменение разработчиком данных игры на модерации {@PendingGameEditData}", new
                {
                    logData.PendingGameId,
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
                _logger.LogError(ex, "Ошибка изменения разработчиком игры на модерации {@PendingGameEditData}", new
                {
                    logData.PendingGameId,
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
