using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LingvoGameOs.Db
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<LanguageLevel> LanguageLevels { get; set; }
        public DbSet<Technology> Technologys { get; set; }
        public DbSet<Platform> Platforms { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Author)
                .WithMany(a => a.DevGames)
                .HasForeignKey(g => g.AuthorId);

            // Настройка связи многие-ко-многим
            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameTypes)
                .WithMany(gt => gt.Games)
                .UsingEntity<GameGameType>(
        j => j
            .HasOne(gt => gt.GameType)  // Настраиваем связь с GameType
            .WithMany()
            .HasForeignKey(gt => gt.GameTypeId),  // Явно указываем внешний ключ

        j => j
            .HasOne(g => g.Game)  // Настраиваем связь с Game
            .WithMany()
            .HasForeignKey(g => g.GameId),  // Явно указываем внешний ключ

        j => j
            .ToTable("GameGameType")
            .HasKey(t => new { t.GameId, t.GameTypeId }) // Составной первичный ключ
    );


            // Инициализация данных по умолчанию
            var languageLevelBeginning = new LanguageLevel { Id = 1, Name = "Начинающий" };
            var languageLevelIntermediate = new LanguageLevel { Id = 2, Name = "Средний" };
            var languageLevelPro = new LanguageLevel { Id = 3, Name = "Продвинутый" };

            modelBuilder.Entity<LanguageLevel>().HasData(
                languageLevelBeginning,
                languageLevelIntermediate,
                languageLevelPro
            );

            var gameType1 = new GameType { Id = 1, Name = "Словарный запас" };
            var gameType2 = new GameType { Id = 2, Name = "Грамматика" };
            var gameType3 = new GameType { Id = 3, Name = "Аудирование" };
            var gameType4 = new GameType { Id = 4, Name = "Чтение" };
            var gameType5 = new GameType { Id = 5, Name = "Говорение" };
            var gameType6 = new GameType { Id = 6, Name = "Головоломка" };
            modelBuilder.Entity<GameType>().HasData(
                gameType1,
                gameType2,
                gameType3,
                gameType4,
                gameType5,
                gameType6
            );


            var platform1 = new Platform { Id = 1, Name = "Web-Desktop" };
            var platform2 = new Platform { Id = 2, Name = "Desktop" };
            var platform3 = new Platform { Id = 3, Name = "Web-Mobile" };
            modelBuilder.Entity<Platform>().HasData(
                platform1,
                platform2,
                platform3
            );

            // Добавление двух игр
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Title = "Горный лабиринт",
                    Rules = "Правила еще находятся в разработке. Простите за неудобства.",
                    Description = "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.",
                    AuthorId = "23691240-2d4a-4354-8d9a-41e6fd99c8f7",
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform2.Id,
                    RaitingPlayers = 4.6,
                    RaitingTeachers = 4.8,
                    CoverImageURL = "/img/games/mountain labyrinth-banner.png",
                    GameURL = "/home/index",
                    NumberDownloads = 1000
                },
                new Game
                {
                    Id = 2,
                    Title = "Тур-викторина 'Арт объекты Осетии'",
                    Rules = "Слушайте гида и выбирайте правильные ответы на его вопросы. Изначально у всех участников 50 баллов, но за неправильный ответ снимают 5 баллов.",
                    Description = "Супер интересная викторина для компании. Поможет найти арт пространства и расскажет о них много интересного.",
                    AuthorId = "23691240-2d4a-4354-8d9a-41e6fd99c8f7",
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform1.Id,
                    RaitingPlayers = 4.4,
                    RaitingTeachers = 4,
                    CoverImageURL = "/img/games/art-object-banner.png",
                    GameURL = "/home/index",
                    NumberDownloads = 241
                },
                new Game
                {
                    Id = 3,
                    Title = "Собери животное",
                    Rules = "Собирайте животное, выбирая правильное название части тела на осетинском языке. За неправильные ответы вы теряете 5 очков. Когда животное собрано, требуется написать его название. Буква 'æ' считается как 2 символа (писать: 'ае').",
                    Description = "Игра состоит из двух уровней никак не связанных друг с другом. После открытия сайта пользователь попадает на главное окно. Там он может ознакомится с правилами игры, а также просмотреть список лидеров и увидеть свой уровень достижений И зарегистрироваться/войти в аккаунт.",
                    AuthorId = "23691240-2d4a-4354-8d9a-41e6fd99c8f7",
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelIntermediate.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 4.2,
                    RaitingTeachers = 4.3,
                    CoverImageURL = "/img/games/gameplay-animal.png",
                    GameURL = "http://84.201.144.125:5001",
                    NumberDownloads = 5
                },
                new Game
                {
                    Id = 4,
                    Title = "Кроссворд осетинских слов",
                    Rules = "Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец.",
                    Description = "Используется текстовое, звуковое и графическое представление языка. Эта игра может быть использована как преподавателями осетинского языка в рамках учебного процесса, так и широким кругом пользователей просто для развлечения.\nНа верхней части страницы находиться кроссворд, который образован из множества вертикальных линий из квадратов, создающие в центре другую линию из квадратов. Каждая из колонок кроссворда помечена цифрой Под кроссвордом находятся вопросы на русском языке, где ответом является слово на осетинском. Это слово необходимо ввести в соответствующий номеру вопроса столбец. После ответа на все вопросы в центре кроссворда на выделенной строке составляется слово на русском языке. В ответ нужно ввести это слово, но на осетинском языке.",
                    AuthorId = "aacde62d-a630-45a1-8ee5-dea31270329c",
                    PublicationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    LanguageLevelId = languageLevelBeginning.Id,
                    GamePlatformId = platform3.Id,
                    RaitingPlayers = 5,
                    RaitingTeachers = 4.9,
                    CoverImageURL = "/img/games/93a62f0945389b9_920x0.jpg",
                    GameURL = "https://ossetian-crosswords.glitch.me/",
                    NumberDownloads = 10
                }
            );


            // Привязка типов игр к играм
            modelBuilder.Entity<GameGameType>().HasData(
                new { GameId = 1, GameTypeId = 2 },
                new { GameId = 2, GameTypeId = 3 },
                new { GameId = 1, GameTypeId = 1 },
                new { GameId = 2, GameTypeId = 4 },
                new { GameId = 3, GameTypeId = 1 },
                new { GameId = 3, GameTypeId = 3 },
                new { GameId = 4, GameTypeId = 1 },
                new { GameId = 4, GameTypeId = 2 },
                new { GameId = 4, GameTypeId = 4 }
            );
        }
    }
}
