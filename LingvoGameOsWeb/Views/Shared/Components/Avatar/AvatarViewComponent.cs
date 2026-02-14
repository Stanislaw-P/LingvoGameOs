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
        readonly S3Service _s3Service;

        public AvatarViewComponent(UserManager<User> userManager, S3Service s3Service)
        {
            _usersManager = userManager;
            _s3Service = s3Service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _usersManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                ViewBag.AvatarImgPath = _s3Service.GetPublicUrl(currentUser.AvatarImgPath);
                ViewBag.Name = currentUser.Name;
                return View("Avatar");
            }
            return View("Avatar");
        }
    }
}
