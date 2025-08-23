using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Text.Json;

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

        public PendingGamesController(UserManager<User> userManager, IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IGamesRepository gamesRepository)
        {
            _userManager = userManager;
            _pendingGamesRepository = pendingGamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _gamesRepository = gamesRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Publish(int gameId)
        {
            PendingGame? pendingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (pendingGame == null)
                return NotFound();

            var publishedGame = await _pendingGamesRepository.PublishAsync(pendingGame);
            // Переносим файлы
            await _fileProvider.MoveGameFilesAsync(
                pendingGame.Id,
                publishedGame.Id,
                Folders.PendingGames,
                Folders.Games);

            // Обновляем пути к файлам
            publishedGame.CoverImagePath = _fileProvider.UpdateFilePath(
                pendingGame.CoverImagePath!,
                $"{Folders.PendingGames}/{pendingGame.Id}",
                $"{Folders.Games}/{publishedGame.Id}");

            if (publishedGame.GamePlatform.Name == "Desktop")
            {
                publishedGame.GameURL = _fileProvider.UpdateFilePath(
                pendingGame.GameURL!,
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

            return Ok(new
            {
                success = true,
                gameData = new
                {
                    id = publishedGame.Id,
                    title = publishedGame.Title,
                    authorName = $"{publishedGame.Author.Name} {publishedGame.Author.Surname}",
                    publicationDate = publishedGame.PublicationDate.ToString("dd.MM.yyyy"),
                    imagePath = publishedGame.CoverImagePath,
                    gameUrl = publishedGame.GameURL,
                    platform = publishedGame.GamePlatform.Name
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendFeedback([FromBody] FeedBackViewModel feedBackView)
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
            try
            {
                var existingGame = await _pendingGamesRepository.TryGetByIdAsync(feedBackView.GameId);
                if (existingGame == null)
                    return NotFound(new { error = $"Игра с id: {feedBackView.GameId} не найдена!" });
                existingGame.LastMessage = feedBackView.Message;

                //TODO: Тут нужно добавить отправку на email разработчика сообщения

                await _pendingGamesRepository.UpdateAsync(existingGame);
                return Ok(new
                {
                    success = true,
                    message = "Сообщение отправлено разработчику"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public async Task<IActionResult> GetGameInfo(int gameId)
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
                GameFolderName = existingGame.GameFolderName,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl ?? "Video doesn't exist",
                LastMessage = existingGame.LastMessage,
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
                existingGame.VideoUrl = editGame.VideoUrl;
                existingGame.GameFolderName = editGame.GameFolderName;

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
