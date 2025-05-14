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


            // Создание разрабов по умолчанию
            var dev1Email = "MaraTest@mail.ru";
            if(userManager.FindByEmailAsync(dev1Email).Result == null)
            {
                var dev1 = new User
                {
                    Id = "23691240-2d4a-4354-8d9a-41e6fd99c8f7", // Этот айди будет у игр Марата
                    Name = "Марат",
                    Surname = "Дзиов",
                    ImageURL = "/img/avatars/MaraAva.jpg",
                    Description = "Выпускниц яндекс лицея. В совершенстве знает python и даже был в офисе Яндекс.",
                    UserName = dev1Email,
                    Email = dev1Email
                };
                var result = userManager.CreateAsync(dev1, password).Result;
                if (result.Succeeded)
                    userManager.AddToRoleAsync(dev1, Constants.DevRoleName).Wait();
            }

            var dev2Email = "DavaTest@mail.ru";
            if(userManager.FindByEmailAsync(dev2Email).Result == null)
            {
                var dev2 = new User
                {
                    Id = "aacde62d-a630-45a1-8ee5-dea31270329c",
                    Name = "Дэвид",
                    Surname = "Кадиев",
                    ImageURL = "/img/avatars/DavaAva.jpg",
                    Description = "Лучший составитель тестов и кроссвордов. Сдал ЕГЭ по информатике на 98 баллов и теперь пишет игры для Осетии.",
                    UserName = dev2Email,
                    Email = dev2Email
                };
                var result = userManager.CreateAsync(dev2, password).Result;
                if (result.Succeeded)
                    userManager.AddToRoleAsync(dev2, Constants.DevRoleName).Wait();
            }
            
        }
    }
}
