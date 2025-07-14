using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class HomeController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly IGamesRepository _gamesRepository;

        public HomeController(UserManager<User> userManager, IGamesRepository gamesRepository)
        {
            _userManager = userManager;
            _gamesRepository = gamesRepository;
        }

        public async Task<IActionResult> Index()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
                return NotFound();

            var existingGames = await _gamesRepository.GetAllAsync();
            var adminUserViewModel = new AdminViewModel { Id = adminUser.Id , Name = adminUser.Name, Surname = adminUser.Surname, UserName  = adminUser.UserName!, Description = adminUser.Description, AvatarImgPath = adminUser.AvatarImgPath, DevGames = existingGames };

            return View(adminUserViewModel);
        }
    }
}
