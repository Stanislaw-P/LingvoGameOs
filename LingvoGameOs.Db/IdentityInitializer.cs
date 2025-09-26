using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace LingvoGameOs.Db
{
    public class IdentityInitializer
    {
        public static async Task InitializeAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            DbContextOptions<DatabaseContext> dbContextOptions,
            IConfiguration configuration
        )
        {
            using (DatabaseContext context = new DatabaseContext(dbContextOptions))
            {
                // создаем роли, если их нет
                await _CreateRole(roleManager, Constants.AdminRoleName);
                await _CreateRole(roleManager, Constants.PlayerRoleName);
                await _CreateRole(roleManager, Constants.DevRoleName);

                // инициализация пользователей
                string passwordDev = configuration["DEVELOPER_USER_PASSWORD"]
                    ?? throw new InvalidOperationException("DEVELOPER_USER_PASSWORD environment variable is required for developer user creation."); ;
                string passwordAdmin = configuration["ADMIN_USER_PASSWORD"]
                    ?? throw new InvalidOperationException("ADMIN_USER_PASSWORD environment variable is required for admin user creation.");

                var adminUser = new User
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    AvatarImgPath = "/Avatars/admin-avatar-11.png",
                    Name = "Админ",
                    Surname = "Админов",
                    Description = "Администрация сайта",
                };

                var devMarat = new User
                {
                    Name = "Марат",
                    Surname = "Дзиов",
                    AvatarImgPath = "/Avatars/MaraAva.jpg",
                    Description =
                        "Выпускниц яндекс лицея. В совершенстве знает python и даже был в офисе Яндекс.",
                    UserName = "MaratTest@gmail.com",
                    Email = "MaratTest@gmail.com",
                };

                var devDavid = new User
                {
                    Name = "Дэвид",
                    Surname = "Кадиев",
                    AvatarImgPath = "/Avatars/DavaAva.jpg",
                    Description =
                        "Лучший составитель тестов и кроссвордов. Сдал ЕГЭ по информатике на 98 баллов и теперь пишет игры для Осетии.",
                    UserName = "DavidTest@gmail.com",
                    Email = "DavidTest@gmail.com",
                };

                var devIlona = new User
                {
                    Name = "Илона",
                    Surname = "Бекоева",
                    AvatarImgPath = "/Avatars/Ilona-ava933.jpg",
                    Description =
                        "Окончила СОГУ с красным дипломом. Вложила душу в проект платформы. Обладательница премии 'Лучшая игра на WinForms' в 2022 году.",
                    UserName = "IlonaBekoeva@gmail.com",
                    Email = "IlonaBekoeva@gmail.com",
                };

                var devNastya = new User
                {
                    Name = "Анастасия",
                    Surname = "Чувенкова",
                    AvatarImgPath = "/Avatars/anastasia-ava.jpg",
                    Description = "Ученица яндекс лицея.",
                    UserName = "AnastasiaChuvenkova@gmail.com",
                    Email = "AnastasiaChuvenkova@gmail.com",
                };

                var devVlad = new User
                {
                    Name = "Владислав",
                    Surname = "Петров",
                    AvatarImgPath = "/Avatars/VslavAva.jpg",
                    Description = "Разработчик игр",
                    UserName = "VladislavPetrov@gmail.com",
                    Email = "VladislavPetrov@gmail.com",
                };
                // создаем пользователя, если его нет
                await _CreateUserAsync(userManager, adminUser, passwordAdmin, Constants.AdminRoleName);
                await _CreateUserAsync(userManager, devMarat, passwordDev, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devDavid, passwordDev, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devIlona, passwordDev, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devNastya, passwordDev, Constants.DevRoleName);
                await _CreateUserAsync(userManager, devVlad, passwordDev, Constants.DevRoleName);

                // Далее инициализация и добавление в БД данных
                var languageLevelBeginning = new LanguageLevel
                {
                    Id = 1,
                    Name = "Барашек, который пытается говорить",
                    Description =
                        "«Барашек, который пытается говорить» – начинаешь издавать осмысленные звуки, но пока не всё понятно.",
                };
                var languageLevelIntermediate = new LanguageLevel
                {
                    Id = 2,
                    Name = "Юный нарт",
                    Description =
                        "«Юный нарт» – уже умеешь составлять предложения и понимаешь базовые правила языка.",
                };
                var languageLevelAdvanced = new LanguageLevel
                {
                    Id = 3,
                    Name = "Кавказский орёл",
                    Description =
                        "«Кавказский орёл» – уверенно говоришь, строишь сложные фразы и можешь поддержать разговор.",
                };
                var languageLevelPro = new LanguageLevel
                {
                    Id = 4,
                    Name = "Старейшина, который говорит тосты",
                    Description =
                        "«Старейшина, который говорит тосты» – свободно владеешь языком, понимаешь тонкости и культурные нюансы, можешь красиво говорить и даже вести застольные беседы.",
                };
                var languageLevelGrandMaster = new LanguageLevel
                {
                    Id = 5,
                    Name = "Хранитель языка",
                    Description =
                        "«Хранитель языка» – ты достиг вершины мастерства, твой язык – как песня гор, а слова – как мудрость веков.",
                };
                if (!await context.LanguageLevels.AnyAsync())
                {
                    context.AddRange(
                        languageLevelBeginning,
                        languageLevelIntermediate,
                        languageLevelAdvanced,
                        languageLevelPro,
                        languageLevelGrandMaster
                    );
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
                    await context.SkillsLearning.AddRangeAsync(
                        gameType1,
                        gameType2,
                        gameType3,
                        gameType4,
                        gameType5,
                        gameType6
                    );
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
                    Title = "Мир по кусочкам",
                    Description = "Узнавайте новые слова и практикуйтесь в языке, складывая пазл.",
                    Rules =
                        "Игра состоит из двух уровней никак не связанных друг с другом.Собирайте животное, выбирая правильное название части тела на осетинском языке. За неправильные ответы вы теряете 5 очков. Когда животное собрано, требуется написать его название. Буква 'æ' считается как 2 символа (писать: 'ае').",
                    GameFolderName = "lingvo-piece-by-piece",
                    AuthorId = devMarat.Id,
                    PublicationDate = new DateTime(2025, 4, 14),
                    LastUpdateDate = new DateTime(2025, 4, 20),
                    LanguageLevelId = languageLevelIntermediate.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 4.2,
                    RaitingTeachers = 4.3,
                    CoverImagePath = "/img/games/gameplay-animal.png",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/gameplay-animal.png",
                        "/img/games/gameplay-animal-1.jpg",
                        "/img/games/gameplay-animal-2.jpg",
                    },
                    // GameURL = "http://158.160.104.26:5001",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType3 },
                    NumberDownloads = 5,
                    Port = 3001,
                };

                var game2 = new Game
                {
                    Id = 2,
                    Title = "Кроссворд осетинских слов",
                    Description =
                        "Кроссворд на осетинском языке, разработанный по последнему писку моды.",
                    Rules =
                        "На верхней части страницы находиться кроссворд, который образован из множества вертикальных линий из квадратов, создающие в центре другую линию из квадратов. Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец. После ответа на все вопросы в центре кроссворда на выделенной строке составляется слово на русском языке. В ответ нужно ввести это слово, но на осетинском языке.\n"
                        + "Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец.",
                    GameFolderName = "crossword-ossetia",
                    AuthorId = devDavid.Id,
                    PublicationDate = new DateTime(2025, 4, 27),
                    LastUpdateDate = new DateTime(2025, 5, 1),
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 5,
                    RaitingTeachers = 4.9,
                    CoverImagePath = "/img/games/93a62f0945389b9_920x0.jpg",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/93a62f0945389b9_920x0.jpg",
                        "/img/games/cross-1.jpg",
                    },
                    // GameURL = "http://158.160.104.26:5002",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType2, gameType4 },
                    NumberDownloads = 10,
                    Port = 3002,
                };

                var game3 = new Game
                {
                    Id = 3,
                    Title = "Ребусы на осетинском",
                    Description =
                        "Игра развивает логическое мышление, расширяет словарный запас и предоставляет возможность для развлечения.",
                    Rules =
                        "Отгадайте ребус, используя подсказки. Для ввода ответа используйте кнопки с буквами осетинского алфавита. У вас есть 3 подсказки для каждого ребуса. За каждый правильный ответ вы получаете 20 очков.",
                    GameFolderName = "linvgo-puzzles_in_ossetian",
                    AuthorId = devNastya.Id,
                    PublicationDate = new DateTime(2025, 5, 10),
                    LastUpdateDate = new DateTime(2025, 5, 25),
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 0,
                    RaitingTeachers = 0,
                    CoverImagePath = "/img/games/puzzle-1.jpg",
                    ImagesPaths = new List<string>
                    {
                        "/img/games/puzzle-1.jpg",
                        "/img/games/puzzle-2.png",
                        "/img/games/puzzle-3.jpg",
                    },
                    // GameURL = "http://158.160.104.26:5003",
                    SkillsLearning = new List<SkillLearning> { gameType1, gameType2, gameType4 },
                    NumberDownloads = 0,
                    Port = 3003,
                };

                var game4 = new Game
                {
                    Id = 4,
                    Title = "Поезд",
                    Description =
                        "Игра развивает память, а также навыки чтения и произношения. В игре доступны три языка: осетинский, грузинский и армянский, а также три уровня сложности.",
                    Rules =
                        "Цель игры: Изучить осетинский, грузинский или армянский язык, сопоставляя слова с картинками и зарабатывая баллы. Успешное завершение уровней повышает рейтинг.\r\nРежимы игры:\r\nУчить слова: Изучайте слова с картинками и озвучкой. Нажмите на картинку, чтобы услышать произношение и запомнить.\r\nИграть: Перетаскивайте картинки в вагоны поезда, подписанные на выбранном языке.\r\nУровни сложности:\r\n1 уровень: 8 вагонов, картинки с подписями на русском, озвучка при нажатии.\r\n2 уровень: 12 вагонов, картинки с подписями на русском, без озвучки.\r\n3 уровень: 19 вагонов, только картинки, без подписей и озвучки.",
                    AuthorId = devIlona.Id,
                    PublicationDate = new DateTime(2025, 9, 3),
                    LastUpdateDate = new DateTime(2025, 9, 3),
                    LanguageLevelId = languageLevelAdvanced.Id,
                    GamePlatformId = platform2.Id,
                    RaitingPlayers = 4.0,
                    RaitingTeachers = 3.7,
                    CoverImagePath = "/Games/4/train-cover.png",
                    ImagesPaths = new List<string>
                    {
                        "/Games/4/train scrin-1.jpg",
                        "/Games/4/train scrin-2.jpg",
                        "/Games/4/train scrin-3.jpg",
                    },
                    SkillsLearning = new List<SkillLearning> { gameType4, gameType5 },
                    NumberDownloads = 4,
                    GameFilePath = "/Games/4/Поезд.msi",
                    VideoUrl =
                        "https://vk.com/away.php?to=https%3A%2F%2Frutube.ru%2Fplay%2Fembed%2Fa0e3032961efb0ca214a35ef3ed9caea&utf=1",
                };

                if (!await context.Games.AnyAsync())
                {
                    await context.AddRangeAsync(game1, game2, game3, game4);
                    await context.SaveChangesAsync();
                }

                var review1 = new Review
                {
                    AuthorId = devDavid.Id,
                    GameId = game1.Id,
                    Rating = 4,
                    PublicationDate = new DateTime(2025, 7, 24),
                    Text =
                        "Игра прикольная! Как интересно собирается картинка, мне интересно как разработчику. Иногда задания кажутся сложноватыми, но это только подогревает интерес!",
                    IsApproved = true,
                };
                var review2 = new Review
                {
                    AuthorId = devMarat.Id,
                    GameId = game2.Id,
                    Rating = 5,
                    PublicationDate = new DateTime(2025, 8, 4),
                    Text =
                        "Мне понравилось, что игры разделены на категории и есть подробности об игре. При этом оформление очень красивое, я как пользователь и как разработчик кайфую. Самое моё любимое это прозвища в профиле, которые за баллы улучшаются.",
                    IsApproved = true,
                };
                var review3 = new Review
                {
                    AuthorId = devVlad.Id,
                    GameId = game3.Id,
                    Rating = 4,
                    PublicationDate = new DateTime(2025, 9, 1),
                    Text =
                        "Очень сложная игра, подсказки тоже не помогли. Хоть головоломки и заставляют думать, но мне в нее пока играть рано.",
                    IsApproved = true,
                };

                if (!await context.Reviews.AnyAsync())
                {
                    await context.Reviews.AddRangeAsync(review1, review2, review3);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task _CreateRole(
            RoleManager<IdentityRole> roleManager,
            string roleName
        )
        {
            // создаем роль, если ее нет
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task _CreateUserAsync(
            UserManager<User> userManager,
            User user,
            string pass,
            string roleName
        )
        {
            if (await userManager.FindByEmailAsync(user.Email!) == null)
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
