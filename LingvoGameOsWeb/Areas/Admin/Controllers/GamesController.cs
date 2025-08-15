using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection.Metadata;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class GamesController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly IGamesRepository _gamesRepository;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly FileProvider _fileProvider;

        public GamesController(UserManager<User> userManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
        }

        public async Task<IActionResult> Index()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
                return NotFound();

            var existingGames = await _gamesRepository.GetAllAsync();
            var pendingGames = await _pendingGamesRepository.GetAllAsync();
            var adminUserViewModel = new AdminViewModel
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Surname = adminUser.Surname,
                UserName = adminUser.UserName!,
                Description = adminUser.Description,
                AvatarImgPath = adminUser.AvatarImgPath,
                ExistingDevGames = existingGames,
                PendingGames = pendingGames
            };

            return View(adminUserViewModel);
        }

        public async Task<IActionResult> PendingDetails(int gameId)
        {
            var existingGame = await _pendingGamesRepository.TryGetByIdAsync(gameId);
            if (existingGame == null)
                return NotFound();

            var skillLearnings = await _skillsLearningRepository.GetAllAsync();

            FileInfo? msiFileInfo = null;
            if (existingGame.GamePlatform.Name == "Desktop")
            {
                msiFileInfo = new FileInfo(_fileProvider.GetGameFileFullPath(existingGame.GameURL ?? ""));
            }

            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View(new EditGameViewModel
            {
                Id = existingGame.Id,
                Title = existingGame.Title,
                Description = existingGame.Description,
                Rules = existingGame.Rules,
                CurrentCoverImage = existingGame.CoverImagePath,
                CurrentImagesPaths = existingGame.ImagesPaths,
                SkillsLearning = existingGame.SkillsLearning.Select(x => x.Name).ToList(),
                Author = existingGame.Author,
                DispatchDate = existingGame.DispatchDate,
                GameURL = existingGame.GameURL,
                GameFileInfo = msiFileInfo,
                GamePlatform = existingGame.GamePlatform.Name,
                LanguageLevel = existingGame.LanguageLevel.Name,
                VideoUrl = existingGame.VideoUrl ?? "Video doesn't exist"
            });
        }
    }
}
