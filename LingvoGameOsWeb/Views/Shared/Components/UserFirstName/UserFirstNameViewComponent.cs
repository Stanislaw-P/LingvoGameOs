using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Views.Shared.Components.UserFirstName
{
    public class UserFirstNameViewComponent : ViewComponent
    {

        readonly UserManager<User> _usersManager;

        public UserFirstNameViewComponent(UserManager<User> userManager)
        {
            _usersManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _usersManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                string fullName = $"{currentUser.Name} {currentUser.Surname}";
                return Content(fullName);
            }
            return Content("Пользователь");
        }
    }
}
