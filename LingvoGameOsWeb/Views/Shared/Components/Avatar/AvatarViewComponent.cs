using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Views.Shared.Components.Avatar
{
    public class AvatarViewComponent : ViewComponent
    {
        readonly UserManager<User> _usersManager;
        readonly IFileStorage _fileStorage;

        public AvatarViewComponent(UserManager<User> userManager, IFileStorage fileStorage)
        {
            _usersManager = userManager;
            _fileStorage = fileStorage;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _usersManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                ViewBag.AvatarImgPath = _fileStorage.GetPublicUrl(currentUser.AvatarImgPath);
                ViewBag.Name = currentUser.Name;
                return View("Avatar");
            }
            return View("Avatar");
        }
    }
}
