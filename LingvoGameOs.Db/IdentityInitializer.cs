using Microsoft.AspNetCore.Identity;
using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
    public class IdentityInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DbContextOptions<DatabaseContext> dbContextOptions)
        {
            using (DatabaseContext context = new DatabaseContext(dbContextOptions))
            {
                // создаем роли, если их нет
                await _CreateRole(roleManager, Constants.AdminRoleName);
                await _CreateRole(roleManager, Constants.PlayerRoleName);
                await _CreateRole(roleManager, Constants.DevRoleName);

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

                var devIlona = new User
                {
                    Name = "Илона",
                    Surname = "Бекоева",
                    ImageURL = "/img/avatar100.png",
                    Description = "Окончила СОГУ с красным дипломом. Вложила душу в проект платформы. Обладательница премии 'Лучшая игра на WinForms' в 2022 году.",
                    UserName = "IlonaBekoeva@gmail.com",
                    Email = "IlonaBekoeva@gmail.com"
                };

                // создаем пользователя, если его нет
                await _CreateUserAsync(userManager, adminUser, password, Constants.AdminRoleName);
                await _CreateUserAsync(userManager, devMarat, password, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devDavid, password, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devIlona, password, Constants.DevRoleName);

                // Далее инициализация и добавление в БД данных
                var languageLevelBeginning = new LanguageLevel { Id = 1, Name = "Барашек, который пытается говорить", Description = "«Барашек, который пытается говорить» – начинаешь издавать осмысленные звуки, но пока не всё понятно." };
                var languageLevelIntermediate = new LanguageLevel { Id = 2, Name = "Юный нарт", Description = "«Юный нарт» – уже умеешь составлять предложения и понимаешь базовые правила языка." };
                var languageLevelAdvanced = new LanguageLevel { Id = 3, Name = "Кавказский орёл", Description = "«Кавказский орёл» – уверенно говоришь, строишь сложные фразы и можешь поддержать разговор." };
                var languageLevelPro = new LanguageLevel { Id = 4, Name = "Старейшина, который говорит тосты", Description = "«Старейшина, который говорит тосты» – свободно владеешь языком, понимаешь тонкости и культурные нюансы, можешь красиво говорить и даже вести застольные беседы." };
                var languageLevelGrandMaster = new LanguageLevel { Id = 5, Name = "Хранитель языка", Description = "«Хранитель языка» – ты достиг вершины мастерства, твой язык – как песня гор, а слова – как мудрость веков." };
                if (!await context.LanguageLevels.AnyAsync())
                {
                    context.AddRange(languageLevelBeginning, languageLevelIntermediate, languageLevelAdvanced, languageLevelPro, languageLevelGrandMaster);
                    await context.SaveChangesAsync();
                }

                var gameType1 = new SkillLearning { Id = 1, Name = "Словарный запас" };
                var gameType2 = new SkillLearning { Id = 2, Name = "Грамматика" };
                var gameType3 = new SkillLearning { Id = 3, Name = "Аудирование" };
                var gameType4 = new SkillLearning { Id = 4, Name = "Чтение" };
                var gameType5 = new SkillLearning { Id = 5, Name = "Говорение" };
                var gameType6 = new SkillLearning { Id = 6, Name = "Диктант" };
                if (!context.SkillsLearning.Any())
                {
                    await context.SkillsLearning.AddRangeAsync(gameType1, gameType2, gameType3, gameType4, gameType5, gameType6);
                    await context.SaveChangesAsync();
                }

                var platform1 = new Platform { Id = 1, Name = "Web-Desktop" };
                var platform2 = new Platform { Id = 2, Name = "Desktop" };
                var platform3 = new Platform { Id = 3, Name = "Web-Mobile" };
                if (!await context.Platforms.AnyAsync())
                {
                    await context.Platforms.AddRangeAsync(platform1, platform2, platform3);
                    await context.SaveChangesAsync();
                }

                var game1 = new Game
                {
                    Id = 1,
                    Title = "Горный лабиринт",
                    Description = "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.",
                    Rules = "Есть 10 уровней. На каждом из которых будут распологаться ловушки и монетки. За сбор 15 монет, вам открывается переход на следующий уровень. Но будьте внимательны! Ведь вам дано только 3 жизни, израсходовав которые, все начинается снова.",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform2.Id,
                    RaitingPlayers = 4.6,
                    RaitingTeachers = 4.8,
                    CoverImagePath = "/img/games/mountain-labyrinth-banner-1.png",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/mountain-labyrinth-banner-1.png",
                        "/img/games/mountain-labyrinth-banner-2.png",
                        "/img/games/mountain-labyrinth-banner-3.png"
                    },
                    GameURL = "/home/index",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType2 },
                    NumberDownloads = 1000
                };

                var game2 = new Game
                {
                    Id = 2,
                    Title = "Тур-викторина 'Арт объекты Осетии'",
                    Description = "Супер интересная викторина для компании. Узнайте популярные туристические объекст гор Осетии в игровой форме.",
                    Rules = "Слушайте гида и выбирайте правильные ответы на его вопросы. Изначально у всех участников 50 баллов, но за неправильный ответ снимают 5 баллов.",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform1.Id,
                    RaitingPlayers = 4.4,
                    RaitingTeachers = 4,
                    CoverImagePath = "/img/games/art-object-1.png",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/art-object-1.png",
                        "/img/games/art-object-2.jpeg",
                        "/img/games/art-object-3.png"
                    },
                    GameURL = "/home/index",
                    SkillsLearning = new List<SkillLearning> { gameType3, gameType4 },
                    NumberDownloads = 241
                };
                var game3 = new Game
                {
                    Id = 3,
                    Title = "Мир по кусочкам",
                    Description = "Узнавайте новые слова и практикуйтесь в языке, складывая пазл.",
                    Rules = "Игра состоит из двух уровней никак не связанных друг с другом.Собирайте животное, выбирая правильное название части тела на осетинском языке. За неправильные ответы вы теряете 5 очков. Когда животное собрано, требуется написать его название. Буква 'æ' считается как 2 символа (писать: 'ае').",
                    AuthorId = devMarat.Id,
                    PublicationDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                    LanguageLevelId = languageLevelIntermediate.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 4.2,
                    RaitingTeachers = 4.3,
                    CoverImagePath = "/img/games/gameplay-animal.png",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/gameplay-animal.png",
                        "/img/games/gameplay-animal-1.jpg",
                        "/img/games/gameplay-animal-2.jpg"
                    },
                    GameURL = "http://158.160.104.26:5001",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType3 },
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
                    PublicationDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 5,
                    RaitingTeachers = 4.9,
                    CoverImagePath = "/img/games/93a62f0945389b9_920x0.jpg",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/93a62f0945389b9_920x0.jpg",
                        "/img/games/cross-1.jpg"
                    },
                    GameURL = "https://ossetian-crosswords.glitch.me/",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType2, gameType4 },
                    NumberDownloads = 10
                };

                if (!await context.Games.AnyAsync())
                {
                    await context.AddRangeAsync(game1, game2, game3, game4);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task _CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            // создаем роль, если ее нет
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task _CreateUserAsync(UserManager<User> userManager, User user, string pass, string roleName)
        {
            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                var result = await userManager.CreateAsync(user, pass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}
