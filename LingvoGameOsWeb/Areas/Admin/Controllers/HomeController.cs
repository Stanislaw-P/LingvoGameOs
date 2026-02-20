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
        readonly S3Service _s3Service;

        public HomeController(UserManager<User> userManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository, S3Service s3Service)
        {
            _userManager = userManager;
            _gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
            _s3Service = s3Service;
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
                    CoverImagePath = _s3Service.GetPublicUrl(game.CoverImagePath),
                    GameFilePath = _s3Service.GetPublicUrl(game.GameFilePath!),
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
                    CoverImagePath = _s3Service.GetPublicUrl(pendingGame.CoverImagePath),
                    GameFilePath = _s3Service.GetPublicUrl(pendingGame.GameFilePath!),
                    GamePlatform = pendingGame.GamePlatform,
                    LanguageLevel = pendingGame.LanguageLevel,
                    SkillsLearning = pendingGame.SkillsLearning,
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
                AvatarImgUrl = _s3Service.GetPreSignedFileUrl(adminUser.AvatarImgPath),
                ExistingDevGames = gamesViewModel,
                PendingGames = pendingGamesViewModel,
                InactiveGames = inactiveGamesViewModel,
                NumberDevelopers = numberDevUser
            };

            return View(adminUserViewModel);
        }
    }
}
