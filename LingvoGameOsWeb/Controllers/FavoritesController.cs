using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        readonly IFavoriteGamesRepository _favoriteGamesRepository;
        readonly UserManager<User> _userManager;

        public FavoritesController(IFavoriteGamesRepository favoriteGamesRepository, UserManager<User> userManager)
        {
            _favoriteGamesRepository = favoriteGamesRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var favoritesGamesUser = await _favoriteGamesRepository.GetUserFavoritesAsync(currentUser.Id);
            return View(favoritesGamesUser);
        }
    }
}
