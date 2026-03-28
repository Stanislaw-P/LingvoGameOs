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
    public class HomeController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly IGamesRepository _gamesRepository;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly IFileStorage _fileStorage;

        public HomeController(UserManager<User> userManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository, IFileStorage fileStorage)
        {
            _userManager = userManager;
            _gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> Index()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
                return NotFound();

            var existingGames = await _gamesRepository.GetAllAsync();
            var gamesViewModel = new List<GameViewModel>();

            foreach (var game in existingGames)
            {
                gamesViewModel.Add(new GameViewModel
                {
                    Id = game.Id,
                    Title = game.Title,
                    CoverImagePath = _fileStorage.GetPublicUrl(game.CoverImagePath),
                    GameFilePath = _fileStorage.GetDownloadUrl(game.GameFilePath!, game.Title, ".msi"),
                    GamePlatform = game.GamePlatform,
                    LanguageLevel = game.LanguageLevel,
                    PublicationDate = game.PublicationDate,
                    SkillsLearning = game.SkillsLearning,
                    AverageRaitingPlayers = game.AverageRaitingPlayers,
                    IsActive = game.IsActive,
                });
            }

            var inactiveGamesViewModel = gamesViewModel.Where(game => !game.IsActive).ToList();
            
            var pendingGames = await _pendingGamesRepository.GetAllAsync();
            var pendingGamesViewModel = new List<PendingGameViewModel>();

            foreach (var pendingGame in pendingGames)
            {
                pendingGamesViewModel.Add(new PendingGameViewModel
                {
                    Id = pendingGame.Id,
                    Title = pendingGame.Title,
                    CoverImagePath = _fileStorage.GetPublicUrl(pendingGame.CoverImagePath!),
                    GameFilePath = _fileStorage.GetDownloadUrl(pendingGame.GameFilePath!, pendingGame.Title, ".msi"),
                    GamePlatform = pendingGame.GamePlatform,
                    LanguageLevel = pendingGame.LanguageLevel,
                    SkillsLearning = pendingGame.SkillsLearning,
                    DispatchDate = pendingGame.DispatchDate
                });
            }
            var devUsers = await _userManager.GetUsersInRoleAsync(Constants.DevRoleName);
            var numberDevUser = devUsers.Count;
            var adminUserViewModel = new AdminViewModel
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Surname = adminUser.Surname,
                UserName = adminUser.UserName!,
                Description = adminUser.Description,
                AvatarImgUrl = _fileStorage.GetPublicUrl(adminUser.AvatarImgPath) ?? "/Avatars/AvaNone.jpg",
                ExistingDevGames = gamesViewModel,
                PendingGames = pendingGamesViewModel,
                InactiveGames = inactiveGamesViewModel,
                NumberDevelopers = numberDevUser
            };

            return View(adminUserViewModel);
        }
    }
}
