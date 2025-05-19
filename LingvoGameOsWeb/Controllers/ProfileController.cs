using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        readonly IGamesRepository gamesRepository;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, IGamesRepository gamesRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.gamesRepository = gamesRepository;
        }

        // перевод из юзера в модельку профиля (пусть пока будет здесь)
        //public ProfileViewModel UserToProfileViewModel(User user)
        //{
        //    if (user != null)
        //    {
        //        ProfileViewModel profile = new ProfileViewModel()
        //        {
        //            Name = user.Name,
        //            Surname = user.Surname,
        //            Email = user.Email,
        //            // пока не знаю как ее получить
        //            Role = "Заглушка роли",
        //            Description = user.Description
        //        };
        //        return profile;
        //    }
        //    return new ProfileViewModel();
        //}

        public IActionResult Index()
        {
            return RedirectToAction("Profile");
        }

        public IActionResult Profile(string userId)
        {
            var user = userManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                var games = gamesRepository.TryGetUserDevGames(user);
                user.DevGames = games;
                return View(user);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel profile)
        {
            return View();
        }
    }
}
