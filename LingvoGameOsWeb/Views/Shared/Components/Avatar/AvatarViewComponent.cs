using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Views.Shared.Components.Avatar
{
    public class AvatarViewComponent : ViewComponent
    {
        readonly UserManager<User> _usersManager;

        public AvatarViewComponent(UserManager<User> userManager)
        {
            _usersManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _usersManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                ViewBag.AvatarImgPath = currentUser.AvatarImgPath;
                ViewBag.Name = currentUser.Name;
                return View("Avatar");
            }
            return View("Avatar");
        }
    }
}
