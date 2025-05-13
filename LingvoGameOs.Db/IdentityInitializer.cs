using Microsoft.AspNetCore.Identity;
using LingvoGameOs.Db.Models;


namespace LingvoGameOs.Db
{
    public class IdentityInitializer
    {
        public static void Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminEmail = "admin@gmail.com";
            var password = "Aa123456_";
            var name = "Админ";
            var surname = "Админов";

            // создаем роль админа, если ее нет
            if (roleManager.FindByNameAsync(Constants.AdminRoleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(Constants.AdminRoleName)).Wait();
            }

            // создаем роль юзера, если ее нет
            if (roleManager.FindByNameAsync(Constants.UserRoleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(Constants.UserRoleName)).Wait();
            }

            // создаем роль разработчика, если ее нет
            if (roleManager.FindByNameAsync(Constants.DevRoleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(Constants.DevRoleName)).Wait();
            }

            // создаем админа, если его нет
            if (userManager.FindByNameAsync(adminEmail).Result == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail, Name = name, Surname = surname };
                var result = userManager.CreateAsync(admin, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, Constants.AdminRoleName).Wait();
                }
            }
        }
    }
}
