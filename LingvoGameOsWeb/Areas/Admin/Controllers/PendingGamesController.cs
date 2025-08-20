using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class PendingGamesController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly FileProvider _fileProvider;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;

        public PendingGamesController(UserManager<User> userManager, IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository)
        {
            _userManager = userManager;
            _pendingGamesRepository = pendingGamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
        }

        public async Task<IActionResult> Details(int gameId)
        {
            var existingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileInfo? msiFileInfo = null;
            if (existingGame.GamePlatform.Name == "Desktop")
            {
                if (existingGame.GameURL != null)
                    msiFileInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.GameURL));
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
                DispatchDate = existingGame.DispatchDate,
                GameURL = existingGame.GameURL,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl ?? "Video doesn't exist"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Details(EditGameViewModel editGame)
        {
            try
            {
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(editGame.Id);
                if (existingGame == null) return NotFound($"Игра с Id: {editGame.Id} не найдена :(");

                List<string>? selectedSkills = editGame.SkillsLearning[0].Split(',').ToList();
                List<SkillLearning> skills = await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                var platform = await _platformsRepository.GetExistingPlatformAsync(editGame.GamePlatform);
                var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(editGame.LanguageLevel);

                existingGame.Title = editGame.Title;
                existingGame.Description = editGame.Description;
                existingGame.Rules = editGame.Rules;
                existingGame.SkillsLearning = skills;
                existingGame.GamePlatform = platform!;
                existingGame.LanguageLevel = languageLvl!;
                existingGame.VideoUrl = existingGame.VideoUrl;

                // Если есть новое изображение - меняем
                await ProcessChangeCoverImage(editGame, existingGame);

                // Процесс удаления картинок
                ProcessDeletedImages(editGame, existingGame);

                // Обрабатываем новые картинки
                await ProcessNewImagesAsync(editGame, existingGame);

                // Удаляем файл игры
                ProcessDeleteGameFile(editGame, existingGame);

                // Меняем адрес игры, если он изменился
                ProcessChangeGameURL(editGame, existingGame);

                await _pendingGamesRepository.UpdateAsync(existingGame);
                return Redirect("/Admin/Home/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private static void ProcessChangeGameURL(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GameURL != editGame.CurrentGameURL)
            {
                existingGame.GameURL = editGame.GameURL;
            }
        }

        private async Task ProcessChangeCoverImage(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.CoverImage != null)
            {
                string? coverImagePath = await _fileProvider.SafeImgFileAsync(editGame.CoverImage, Folders.PendingGames, existingGame.Id);
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
                    var deletedImagePath = _fileProvider.GetFileShortPath(deletedImg, Folders.PendingGames, pendingGame.Id);
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

            List<string> newImagesPaths = await _fileProvider.SafeImagesFilesAsync(
                editGameViewModel.UploadedImages.Where(f => f.Length > 0).ToArray(),
                Folders.PendingGames, editGameViewModel.Id);

            if (pendingGame.ImagesPaths == null)
                pendingGame.ImagesPaths = new List<string>();
            pendingGame.ImagesPaths.AddRange(newImagesPaths);
        }

        private void ProcessDeleteGameFile(EditGameViewModel editGame, PendingGame existingGame)
        {
            if (editGame.GamePlatform == "Desktop" && editGame.GameURL == null)
            {
                if (editGame.CurrentGameURL != null)
                {
                    existingGame.GameURL = null;
                    _fileProvider.DeleteFile(editGame.CurrentGameURL);
                }
            }
        }
    }
}
