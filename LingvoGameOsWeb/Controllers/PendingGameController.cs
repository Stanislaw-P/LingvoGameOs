using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class PendingGameController : Controller
    {
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly FileProvider _fileProvider;

        public PendingGameController(IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment webHostEnvironment)
        {
            _pendingGamesRepository = pendingGamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(webHostEnvironment);
        }

        public async Task<IActionResult> EditAsync(int gameId)
        {
            var existingPendingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (existingPendingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileInfo? msiFileInfo = null;
            if (existingPendingGame.GamePlatform.Name == "Desktop")
            {
                // If game msi file path not null
                if (existingPendingGame.GameFilePath != null)
                    msiFileInfo = new FileInfo(_fileProvider.GetFileFullPath(existingPendingGame.GameFilePath));
            }
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View(new AdminEditGameViewModel
            {
                Id = existingPendingGame.Id,
                Title = existingPendingGame.Title,
                Description = existingPendingGame.Description,
                Rules = existingPendingGame.Rules,
                CurrentCoverImagePath = existingPendingGame.CoverImagePath,
                CoverImageInfo = new FileInfo(_fileProvider.GetFileFullPath(existingPendingGame.CoverImagePath)),
                ImagesFilesInfo = _fileProvider.GetImagesFilesInfo(existingPendingGame.ImagesPaths),
                SkillsLearning = existingPendingGame.SkillsLearning.Select(x => x.Name).ToList(),
                Author = existingPendingGame.Author,
                AuthorId = existingPendingGame.Author.Id,
                GameFilePath = existingPendingGame.GameFilePath,
                GameFolderName = existingPendingGame.GameFolderName,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingPendingGame.GamePlatform.Name,
                LanguageLevel = existingPendingGame.LanguageLevel.Name,
                VideoUrl = existingPendingGame.VideoUrl,
                LastUpdateDate = existingPendingGame.LastUpdateDate,
                DispatchDate = existingPendingGame.DispatchDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(AdminEditGameViewModel editGameView)
        {
            if (ModelState.IsValid)
            {

            }
            return View(editGameView);
        }
    }
}
