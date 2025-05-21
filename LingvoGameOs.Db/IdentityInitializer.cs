using Microsoft.AspNetCore.Identity;
using LingvoGameOs.Db.Models;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;


namespace LingvoGameOs.Db
{
    public class IdentityInitializer
    {
        public static void Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DbContextOptions<DatabaseContext> dbContextOptions)
        {
            using (DatabaseContext context = new DatabaseContext(dbContextOptions))
            {
                // создаем роли, если их нет
                _CreateRole(roleManager, Constants.AdminRoleName);
                _CreateRole(roleManager, Constants.PlayerRoleName);
                _CreateRole(roleManager, Constants.DevRoleName);

                // инициализация пользователей
                var password = "Aa123456_";

                var adminUser = new User
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    ImageURL = "/img/avatars/admin-avatar-11.png",
                    Name = "Админ",
                    Surname = "Админов",
                    Description = "Администрация сайта"
                };

                var devMarat = new User
                {
                    Name = "Марат",
                    Surname = "Дзиов",
                    ImageURL = "/img/avatars/MaraAva.jpg",
                    Description = "Выпускниц яндекс лицея. В совершенстве знает python и даже был в офисе Яндекс.",
                    UserName = "MaratTest@gmail.com",
                    Email = "MaratTest@gmail.com"
                };

                var devDavid = new User
                {
                    Name = "Дэвид",
                    Surname = "Кадиев",
                    ImageURL = "/img/avatars/DavaAva.jpg",
                    Description = "Лучший составитель тестов и кроссвордов. Сдал ЕГЭ по информатике на 98 баллов и теперь пишет игры для Осетии.",
                    UserName = "DavidTest@gmail.com",
                    Email = "DavidTest@gmail.com"
                };

                // создаем пользователя, если его нет
                _CreateUser(userManager, adminUser, password, Constants.AdminRoleName);
                _CreateUser(userManager, devMarat, password, Constants.DevRoleName);
                _CreateUser(userManager, devDavid, password, Constants.DevRoleName);


                // Далее инициализация и добавление в БД данных
                var languageLevelBeginning = new LanguageLevel { Id = 1, Name = "Начинающий" };
                var languageLevelIntermediate = new LanguageLevel { Id = 2, Name = "Средний" };
                var languageLevelPro = new LanguageLevel { Id = 3, Name = "Продвинутый" };
                if (!context.LanguageLevels.Any())
                {
                    context.AddRange(languageLevelBeginning, languageLevelIntermediate, languageLevelPro);
                    context.SaveChanges();
                }

                var gameType1 = new GameType { Id = 1, Name = "Словарный запас" };
                var gameType2 = new GameType { Id = 2, Name = "Грамматика" };
                var gameType3 = new GameType { Id = 3, Name = "Аудирование" };
                var gameType4 = new GameType { Id = 4, Name = "Чтение" };
                var gameType5 = new GameType { Id = 5, Name = "Говорение" };
                var gameType6 = new GameType { Id = 6, Name = "Головоломка" };
                if (!context.GameTypes.Any())
                {
                    context.GameTypes.AddRange(gameType1, gameType2, gameType3, gameType4, gameType5, gameType6);
                    context.SaveChanges();
                }

                var platform1 = new Platform { Id = 1, Name = "Web-Desktop" };
                var platform2 = new Platform { Id = 2, Name = "Desktop" };
                var platform3 = new Platform { Id = 3, Name = "Web-Mobile" };
                if (!context.Platforms.Any())
                {
                    context.Platforms.AddRange(platform1, platform2, platform3);
                    context.SaveChanges();
                }

                var game1 = new Game
                {
                    Id = 1,
                    Title = "Горный лабиринт",
                    Description = "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.",
                    Rules = "Есть 10 уровней. На каждом из которых будут распологаться ловушки и монетки. За сбор 15 монет, вам открывается переход на следующий уровень. Но будьте внимательны! Ведь вам дано только 3 жизни, израсходовав которые, все начинается снова.",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform2.Id,
                    RaitingPlayers = 4.6,
                    RaitingTeachers = 4.8,
                    CoverImageURL = "/img/games/mountain labyrinth-banner.png",
                    GameURL = "/home/index",
                    GameTypes = new List<GameType> { gameType1, gameType2 },
                    NumberDownloads = 1000
                };

                var game2 = new Game
                {
                    Id = 2,
                    Title = "Тур-викторина 'Арт объекты Осетии'",
                    Description = "Супер интересная викторина для компании. Узнайте популярные туристические объекст гор Осетии в игровой форме.",
                    Rules = "Слушайте гида и выбирайте правильные ответы на его вопросы. Изначально у всех участников 50 баллов, но за неправильный ответ снимают 5 баллов.",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform1.Id,
                    RaitingPlayers = 4.4,
                    RaitingTeachers = 4,
                    CoverImageURL = "/img/games/art-object-banner.png",
                    GameURL = "/home/index",
                    GameTypes = new List<GameType> { gameType3, gameType4 },
                    NumberDownloads = 241
                };
                var game3 = new Game
                {
                    Id = 3,
                    Title = "Собери животное",
                    Description = "Узнавайте новые слова и практикуйтесь в языке, складывая пазл.",
                    Rules = "Игра состоит из двух уровней никак не связанных друг с другом.Собирайте животное, выбирая правильное название части тела на осетинском языке. За неправильные ответы вы теряете 5 очков. Когда животное собрано, требуется написать его название. Буква 'æ' считается как 2 символа (писать: 'ае').",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelIntermediate.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 4.2,
                    RaitingTeachers = 4.3,
                    CoverImageURL = "/img/games/gameplay-animal.png",
                    GameURL = "http://84.201.144.125:5001",
                    GameTypes = new List<GameType> { gameType1, gameType3 },
                    NumberDownloads = 5
                };
                var game4 = new Game
                {
                    Id = 4,
                    Title = "Кроссворд осетинских слов",
                    Description = "Кроссворд на осетинском языке, разработанный по последнему писку моды.",
                    Rules = "На верхней части страницы находиться кроссворд, который образован из множества вертикальных линий из квадратов, создающие в центре другую линию из квадратов. Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец. После ответа на все вопросы в центре кроссворда на выделенной строке составляется слово на русском языке. В ответ нужно ввести это слово, но на осетинском языке.\n" +
                    "Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец.",
                    AuthorId = devDavid.Id,
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 5,
                    RaitingTeachers = 4.9,
                    CoverImageURL = "/img/games/93a62f0945389b9_920x0.jpg",
                    GameURL = "https://ossetian-crosswords.glitch.me/",
                    GameTypes = new List<GameType> { gameType1, gameType2, gameType4 },
                    NumberDownloads = 10
                };

                if (!context.Games.Any())
                {
                    context.AddRange(game1, game2, game3, game4);
                    context.SaveChanges();
                }
            }
        }

        private static void _CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            // создаем роль, если ее нет
            if (roleManager.FindByNameAsync(roleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            }
        }

        private static void _CreateUser(UserManager<User> userManager, User user, string pass, string roleName)
        {
            if (userManager.FindByEmailAsync(user.Email).Result == null)
            {
                var result = userManager.CreateAsync(user, pass).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, roleName).Wait();
                }
            }
        }
    }
}
