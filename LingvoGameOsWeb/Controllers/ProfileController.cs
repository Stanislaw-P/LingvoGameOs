using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        readonly IGamesRepository gamesRepository;
        readonly IPendingGamesRepository _pendingGamesRepository;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
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


        public async Task<IActionResult> IndexAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var games = await gamesRepository.TryGetUserDevGamesAsync(user);
                user.DevGames = games;
                var pendingGames = await _pendingGamesRepository.TryGetUserDevGamesAsync(user);
                user.DevPendingGames = pendingGames;
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Level = 1,
                    Description = user.Description,
                    AvatarImgPath = user.AvatarImgPath,
                    DevGames = user.DevGames,
                    DevPendingGames = user.DevPendingGames,
                    PlayerGames = user.PlayerGames,
                    UserGames = user.UserGames
                };

                User? UserProfileOwner = await userManager.GetUserAsync(User);
                if (userId == UserProfileOwner?.Id)
                {
                    userViewModel.IsMyProfile = true;
                }

                return View(userViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SettingsAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var games = await gamesRepository.TryGetUserDevGamesAsync(user);
                user.DevGames = games;

                User? UserProfileOwner = await userManager.GetUserAsync(User);
                if (userId == UserProfileOwner?.Id)
                {
                    return View(new UserViewModel() { Id = user.Id, Name = user.Name, Surname = user.Surname, UserName = user.UserName!, Level = 1, Description = user.Description, AvatarImgPath = user.AvatarImgPath, DevGames = user.DevGames, PlayerGames = user.PlayerGames, UserGames = user.UserGames });
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> SettingsAsync(UserViewModel settings)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var games = await gamesRepository.TryGetUserDevGamesAsync(user);
            user.DevGames = games;
            settings.PlayerGames = user.PlayerGames;
            settings.UserGames = user.UserGames;
            settings.DevGames = user.DevGames;
            if (ModelState.IsValid)
            {
                if (user.UserName != settings.UserName)
                {
                    var newEmailUser = await userManager.FindByEmailAsync(settings.UserName);
                    if (newEmailUser == null)
                    {
                        var token = await userManager.GenerateChangeEmailTokenAsync(user, settings.UserName);
                        var result = await userManager.ChangeEmailAsync(user, settings.UserName, token);
                        user.UserName = settings.UserName;
                        user.NormalizedUserName = userManager.NormalizeName(settings.UserName);
                        user.EmailConfirmed = false;
                    }
                    else
                    {
                        ModelState.AddModelError("EmailIsNotAvailable", "Такой email уже используется");
                    }
                }
                if (user.Name != settings.Name)
                {
                    user.Name = settings.Name;
                }
                if (user.Surname != settings.Surname)
                {
                    user.Surname = settings.Surname;
                }
                if (user.Description != settings.Description)
                {
                    user.Description = settings.Description;
                }
                await userManager.UpdateAsync(user);
                await signInManager.RefreshSignInAsync(user);
            }
            return View(settings);
        }

        public async Task<IActionResult> ProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var games = await gamesRepository.TryGetUserDevGamesAsync(user);
                user.DevGames = games;

                var currentUser = await userManager.GetUserAsync(User);
                ViewData["UserImageURL"] = currentUser?.AvatarImgPath; // Передаем URL аватара текущего пользователя
                ViewData["Username"] = currentUser?.UserName ?? "Пользователь"; // Передаем имя

                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Level = 1,
                    Description = user.Description,
                    AvatarImgPath = user.AvatarImgPath,
                    DevGames = user.DevGames,
                    PlayerGames = user.PlayerGames,
                    UserGames = user.UserGames
                };

                if (userId == userManager.GetUserAsync(User).Result.Id)
                {
                    userViewModel.IsMyProfile = true;
                }

                return View(userViewModel);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
