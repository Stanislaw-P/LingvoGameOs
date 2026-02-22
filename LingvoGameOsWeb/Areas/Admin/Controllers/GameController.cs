using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class GameController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly ILogger<GameController> _logger;
        readonly FileProvider _fileProvider;
        readonly UserManager<User> _userManager;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly EmailService _emailService;
        readonly S3Service _s3Service;
        readonly GameFileProcessor _gameFileProcessor;

        public GameController(IGamesRepository gamesRepository, ILogger<GameController> logger, IWebHostEnvironment webHostEnvironment, ILanguageLevelsRepository languageLevelsRepository, ISkillsLearningRepository skillsLearningRepository, UserManager<User> userManager, IPlatformsRepository platformsRepository, EmailService emailService, S3Service s3Service, GameFileProcessor gameFileProcessor)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
            _fileProvider = new FileProvider(webHostEnvironment);
            _languageLevelsRepository = languageLevelsRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _userManager = userManager;
            _platformsRepository = platformsRepository;
            _emailService = emailService;
            _s3Service = s3Service;
            _gameFileProcessor = gameFileProcessor;
        }

        public async Task<IActionResult> DeactivateAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            await _gamesRepository.Deactivate(existingGame!);
            await _emailService.TrySendAboutDeactivateGameAsync(existingGame?.Author.Name ?? "Разработчик", existingGame?.Author?.Email!, existingGame?.Title!);
            return Redirect("/Admin/Home/");
        }

        public async Task<IActionResult> ActivateAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            await _gamesRepository.Activate(existingGame!);
            await _emailService.TrySendAboutPublicationAsync(existingGame?.Author.Name ?? "Разработчик", existingGame?.Author?.Email!, existingGame?.Title!);
            return Redirect("/Admin/Home/");
        }

        public async Task<IActionResult> DeleteInactiveAsync(int gameId)
        {
            var gameForDelete = await _gamesRepository.TryGetByIdAsync(gameId);

            if (gameForDelete == null)
                return NotFound();

            string directoryPendingGamePath = _fileProvider.GetGameDirectoryPath(gameId, Folders.Games);
            _fileProvider.DeleteDirectory(directoryPendingGamePath);

            await _gamesRepository.RemoveAsync(gameForDelete);
            await _emailService.TrySendRefusalGameAsync(gameForDelete.Author.Name, gameForDelete?.Author?.Email!, gameForDelete?.Title!);
            return Redirect("/Admin/Home/");
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
                AuthorId = existingGame?.Author.Id!,
                GameFilePath = existingGame?.GameFilePath,
                GameGitHubUrl = existingGame!.GameGitHubUrl,
                Port = existingGame.Port,
                GameFolderName = existingGame.GameFolderName,
                GameFileMetadata = msiFileMetadata,
                GamePlatform = existingGame?.GamePlatform?.Name!,
                LanguageLevel = existingGame?.LanguageLevel?.Name!,
                VideoUrl = existingGame?.VideoUrl,
                LastUpdateDate = existingGame.LastUpdateDate,
                PublicationDate = existingGame.PublicationDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(AdminEditGameViewModel editGame)
        {
            var adminUserId = _userManager.GetUserId(User);
            var logData = new
            {
                GameId = editGame.Id,
                AdminUserId = adminUserId,
                AdminUserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                AdminUserAgent = Request.Headers.UserAgent.ToString(),
            };

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
                await _gameFileProcessor.ProcessChangeCoverImageAsync(editGame, existingGame, Folders.Games);
                
                // Процесс удаления картинок
                await _gameFileProcessor.ProcessDeletedImagesAsync(editGame, existingGame);
                
                // Обрабатываем новые картинки
                await _gameFileProcessor.ProcessNewImagesAsync(editGame, existingGame, Folders.Games);

                // Удаляем файл игры, если игра стала не Desktop или админ удалил файл для Desktop игры
                await _gameFileProcessor.ProcessDeleteGameFileAsync(editGame, existingGame);

                await _gamesRepository.UpdateAsync(existingGame);

                _logger.LogInformation("Именение данных игры {@GameEditData}", new
                {
                    logData.GameId,
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
                _logger.LogError(ex, "Ошибка изменения игры {@GameEditData}", new
                {
                    logData.GameId,
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
