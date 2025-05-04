using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace LingvoGameOs.Db
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Game> Games { get; set; }
		public DbSet<GameType> GameTypes { get; set; }
		public DbSet<LanguageLevel> LanguageLevels { get; set; }
		public DbSet<Technology> Technologys { get; set; }
		public DbSet<Platform> Platforms { get; set; }
		public DbSet<PlayerUser> PlayerUsers { get; set; }
		public DbSet<DevUser> DevUsers { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
			Database.Migrate();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

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


			DevUser devUser1 = new DevUser { Id = 1, Name = "Марат", Description = "Студент яндекс лицея. Лучший разработчик по версии журнала Babushka", Email = "MaratTest@mail.ru", Login = "MaratTest", Password = "_Aa123456" };

			modelBuilder.Entity<DevUser>().HasData(
				devUser1
			);

			// Добавление двух игр
			// я добавил новое свойство, надо добавить их и здесь
			modelBuilder.Entity<Game>().HasData(
				new Game
				{
					Id = 1,
					Title = "Горный лабиринт",
					Description = "Отправляйтесь в увлекательное путешествие, проходите сказочные лабиринты и создавайте собственные в удобном редакторе.",
					AuthorId = devUser1.Id,
					PublicationDate = DateTime.UtcNow,
					LastUpdateDate = DateTime.UtcNow,
					LanguageLevelId = languageLevelBeginning.Id,
					GamePlatformId = platform2.Id,
					Raiting = 4.6,
					CoverImageURL = "/img/games/mountain labyrinth-banner.png",
					GameURL = "/home/index",
					NumberDownloads = 1000,
					//
					AuthorStringId = ""
				},
				new Game
				{
					Id = 2,
					Title = "Тур-викторина 'Арт объекты Осетии'",
					Description = "Супер интересная викторина для компании. Поможет найти арт пространства и расскажет о них много интересного.",
					AuthorId = devUser1.Id,
					PublicationDate = DateTime.UtcNow,
					LastUpdateDate = DateTime.UtcNow,
					LanguageLevelId = languageLevelBeginning.Id,
					GamePlatformId = platform1.Id,
					Raiting = 4.4,
					CoverImageURL = "/img/games/art-object-banner.png",
					GameURL = "/home/index",
					NumberDownloads = 2241,
					//
                    AuthorStringId = ""
                },
				new Game
				{
					Id = 3,
					Title = "Собери животное",
					Description = "Игра состоит из двух уровней никак не связанных друг с другом. После открытия сайта пользователь попадает на главное окно. Там он может ознакомится с правилами игры, а также просмотреть список лидеров и увидеть свой уровень достижений И зарегистрироваться/войти в аккаунт.",
					AuthorId = devUser1.Id,
					PublicationDate = DateTime.UtcNow,
					LastUpdateDate = DateTime.UtcNow,
					LanguageLevelId = languageLevelIntermediate.Id,
					GamePlatformId = platform3.Id,
					Raiting = 4.2,
					CoverImageURL = "/img/games/gameplay-animal.png",
					GameURL = "http://84.201.144.125:5001",
					NumberDownloads = 5,
					//
                    AuthorStringId = ""
                }
			);


			// Привязка типов игр к играм
			modelBuilder.Entity<GameGameType>().HasData(
				new { GameId = 1, GameTypeId = 2 },
				new { GameId = 2, GameTypeId = 3 },
				new { GameId = 1, GameTypeId = 1 },
				new { GameId = 2, GameTypeId = 4 },
				new { GameId = 3, GameTypeId = 1 },
				new { GameId = 3, GameTypeId = 3 }
			);
		}
	}
}
