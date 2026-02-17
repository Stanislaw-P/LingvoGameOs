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

        public PendingGameController(IPendingGamesRepository pendingGamesRepository, IWebHostEnvironment appEnvironment, EmailService emailService, ISkillsLearningRepository skillsLearningRepository, ILanguageLevelsRepository languageLevelsRepository, IPlatformsRepository platformsRepository, ILogger<PendingGameController> logger, UserManager<User> userManager)
        {
            _pendingGamesRepository = pendingGamesRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _emailService = emailService;
            _skillsLearningRepository = skillsLearningRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _platformsRepository = platformsRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> EditAsync(int pendingGameId)
        {
            var existingGame = await _pendingGamesRepository.TryGetByIdAsync(pendingGameId);
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
                CoverImageInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.CoverImagePath!)),
                ImagesFilesInfo = _fileProvider.GetImagesFilesInfo(existingGame.ImagesPaths!),
                SkillsLearning = existingGame?.SkillsLearning?.Select(x => x.Name).ToList(),
                Author = existingGame?.Author,
                AuthorId = existingGame?.Author?.Id!,
                DispatchDate = existingGame.DispatchDate,
                GameFilePath = existingGame.GameFilePath,
                GameGitHubUrl = existingGame.GameGitHubUrl,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame?.GamePlatform?.Name!,
                LanguageLevel = existingGame?.LanguageLevel?.Name!,
                VideoUrl = existingGame?.VideoUrl,
                LastMessage = existingGame?.LastMessage,
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditGameViewModel editGame)
        {
            if (!ModelState.IsValid)
            {
                var skillLearnings = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);

                var authorGame = await _userManager.FindByIdAsync(editGame.AuthorId);
                if (authorGame != null)
                    editGame.Author = authorGame;

                FileInfo? msiFileInfo = null;
                if (editGame?.GamePlatform == "Desktop")
                {
                    if (editGame.GameFilePath != null)
                        msiFileInfo = new FileInfo(_fileProvider.GetFileFullPath(editGame.GameFilePath));
                }
                editGame!.GameFileInfo = msiFileInfo;

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
                existingGame.LastUpdateDate = DateTimeOffset.UtcNow;
                existingGame.GameGitHubUrl = editGame.GameGitHubUrl;

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

                // Добавляем обработку нового файла игры
                await ProcessNewGameFileAsync(editGame, existingGame);

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

        // Удаляем файл игры если он был удален пользователем или если пользователь изменил платформу с десктоп на Веб
        private void ProcessDeleteGameFile(EditGameViewModel editGame, PendingGame existingGame)
        {
            if ((editGame.GamePlatform == "Desktop" && editGame.GameFilePath == null) || (editGame.GamePlatform != "Desktop" && editGame.GameFilePath != null))
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

        // Добавляем новый метод для обработки нового файла игры
        private async Task ProcessNewGameFileAsync(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GamePlatform == "Desktop" && editGame.UploadedGameFile != null && editGame.UploadedGameFile.Length > 0)
            {

                // Если есть старый файл - удаляем его
                if (!string.IsNullOrEmpty(editGame.CurrentGameFilePath))
                {
                    _fileProvider.DeleteFile(editGame.CurrentGameFilePath);
                }

                // Сохраняем новый файл игры
                string? newGameFilePath = await _fileProvider.SaveGameFileAsync(
                    editGame.UploadedGameFile,
                    existingGame.Id,
                    existingGame.Title,
                    Folders.PendingGames
                );

                // Обновляем путь к файлу в базе данных
                existingGame.GameFilePath = newGameFilePath;
                editGame.GameFilePath = newGameFilePath;
                editGame.CurrentGameFilePath = newGameFilePath;
            }
        }
    }
}
