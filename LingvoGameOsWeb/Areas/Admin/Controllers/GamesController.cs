using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class GamesController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly ILogger<GamesController> _logger;
        readonly FileProvider _fileProvider;
        readonly UserManager<User> _userManager;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;

        public GamesController(IGamesRepository gamesRepository, ILogger<GamesController> logger, IWebHostEnvironment webHostEnvironment, ILanguageLevelsRepository languageLevelsRepository, ISkillsLearningRepository skillsLearningRepository, UserManager<User> userManager, IPlatformsRepository platformsRepository)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
            _fileProvider = new FileProvider(webHostEnvironment);
            _languageLevelsRepository = languageLevelsRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _userManager = userManager;
            _platformsRepository = platformsRepository;
        }

        public async Task<IActionResult> EditAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileInfo? msiFileInfo = null;
            if (existingGame.GamePlatform.Name == "Desktop")
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
                SkillsLearning = existingGame.SkillsLearning.Select(x => x.Name).ToList(),
                Author = existingGame.Author,
                AuthorId = existingGame.Author.Id,
                GameURL = existingGame.GameFilePath,
                GameFolderName = existingGame.GameFolderName,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl ?? "Video doesn't exist",
                LastUpdateDate = existingGame.LastUpdateDate,
                PublicationDate = existingGame.PublicationDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditGameViewModel editGame)
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

                // Меняем адрес игры, если он изменился
                ProcessChangeGameURL(editGame, existingGame);

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

        private void ProcessChangeGameURL(EditGameViewModel editGame, Game existingGame)
        {
            if (editGame.GameURL != editGame.CurrentGameURL)
            {
                existingGame.GameFilePath = editGame.GameURL;
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
            if (editGame.GamePlatform == "Desktop" && editGame.GameURL == null)
            {
                if (editGame.CurrentGameURL != null)
                {
                    existingGame.GameFilePath = null;
                    _fileProvider.DeleteFile(editGame.CurrentGameURL);
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
                    string newGameFilePath = _fileProvider.UpdateFileName(editGame.CurrentGameURL, newGameFileName + ".msi"); 
                    editGame.GameURL = newGameFilePath;
                    existingGame.Title = newGameFileName;
                    _fileProvider.MoveGameFile(existingGame.GameFilePath, editGame.GameURL);
                }
            }
        }
    }
}
