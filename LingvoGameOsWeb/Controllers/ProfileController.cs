using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Helpers;

namespace LingvoGameOs.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        readonly IGamesRepository gamesRepository;
        readonly IPendingGamesRepository _pendingGamesRepository;
        readonly FileProvider fileProvider;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, IGamesRepository gamesRepository, IPendingGamesRepository pendingGamesRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.gamesRepository = gamesRepository;
            _pendingGamesRepository = pendingGamesRepository;
            this.fileProvider = new FileProvider(webHostEnvironment);

        }

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
                User? UserProfileOwner = await userManager.GetUserAsync(User);
                if (userId == UserProfileOwner?.Id)
                {
                    return View(new EditUserViewModel() { Id = user.Id, UserName = user.UserName, Name = user.Name, Surname = user.Surname, Description = user.Description, AvatarImgPath = user.AvatarImgPath });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SettingsAsync(EditUserViewModel settings)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
                        ModelState.AddModelError("", "Такой email уже используется");
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
                if (settings.UploadedFile != null)
                {
                    if (settings.UploadedFile.ContentType.Split("/")[0] == "image")
                    {
                        user.AvatarImgPath = await fileProvider.SaveProfileImgFileAsync(settings.UploadedFile, Folders.Avatars);
                        if (settings.AvatarImgPath != null && settings.AvatarImgPath != "/img/avatar100.png")
                        {
                            fileProvider.DeleteFile(settings.AvatarImgPath);
                        }
                        settings.AvatarImgPath = user.AvatarImgPath;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Загрузите файл изображения");
                    }
                }
                await userManager.UpdateAsync(user);
                await signInManager.RefreshSignInAsync(user);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Profile", new {userId = user.Id});
            }
            return View(settings);
        }
    }
}
