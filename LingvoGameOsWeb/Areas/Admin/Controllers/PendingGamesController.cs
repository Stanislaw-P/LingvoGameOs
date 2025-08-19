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
                msiFileInfo = new FileInfo(_fileProvider.GetFileFullPath(existingGame.GameURL ?? ""));
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
                
                

                return View(platform);

            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        public void DeleteGameImg(string imgPath)
        {
            if (System.IO.File.Exists(imgPath))
                System.IO.File.Delete(imgPath);
        }
    }
}
