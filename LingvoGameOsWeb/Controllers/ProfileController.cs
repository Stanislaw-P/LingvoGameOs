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

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // перевод из юзера в модельку профиля (пусть пока будет здесь)
        public ProfileViewModel UserToProfileViewModel(User user)
        {
            if (user != null)
            {
                ProfileViewModel profile = new ProfileViewModel()
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    // пока не знаю как ее получить
                    Role = "Заглушка роли",
                    Description = user.Description
                };
                return profile;
            }
            return new ProfileViewModel();
        }

        public IActionResult Index(string name)
        {
            return RedirectToAction("EditProfile", new { name });
        }

        public IActionResult EditProfile(string name)
        {
            var user = userManager.FindByNameAsync(name).Result;
            return View(UserToProfileViewModel(user));
        }

        [HttpPost]
        public IActionResult EditProfile(ProfileViewModel profile)
        {
            return View(profile);
        }
    }
}
