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
            var inactiveGames = existingGames.Where(game => !game.IsActive).ToList();
            var pendingGames = await _pendingGamesRepository.GetAllAsync();
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
                ExistingDevGames = existingGames,
                PendingGames = pendingGames,
                InactiveGames = inactiveGames,
                NumberDevelopers = numberDevUser
            };

            return View(adminUserViewModel);
        }
    }
}
