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

        public GameController(IGamesRepository gamesRepository, UserManager<User> userManager, ILogger<GameController> logger, ISkillsLearningRepository skillsLearningRepository, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IWebHostEnvironment webHostEnvironment)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _logger = logger;
            _skillsLearningRepository = skillsLearningRepository;
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _fileProvider = new FileProvider(webHostEnvironment);
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
                GameFilePath = existingGame.GameFilePath,
                GameFolderName = existingGame.GameFolderName,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl,
                LastUpdateDate = existingGame.LastUpdateDate,
                PublicationDate = existingGame.PublicationDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditGameViewModel editGame)
        {
            return View();
        }
    }
}
