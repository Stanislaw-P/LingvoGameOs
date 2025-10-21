using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LingvoGameOs.Db
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<SkillLearning> SkillsLearning { get; set; }
        public DbSet<LanguageLevel> LanguageLevels { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<PendingGame> PendingGames { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FavoriteGame> FavoriteGames { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .Property(g => g.IsActive)
                .HasDefaultValue(true);

            // Настройка связей
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Author)
                .WithMany(a => a.DevGames)
                .HasForeignKey(g => g.AuthorId);

            // Настройка связи многие-ко-многим
            //modelBuilder.Entity<Game>()
            //    .HasMany(g => g.Players)
            //    .WithMany(u => u.PlayerGames)
            //    .UsingEntity<GameHistory>();

            modelBuilder.Entity<Game>()
                .HasMany(g => g.SkillsLearning)
                .WithMany(gt => gt.Games)
                .UsingEntity<GameSkillLearning>(
                    j => j
                        .HasOne(gt => gt.SkillLearning)  // Настраиваем связь с GameType
                        .WithMany()
                        .HasForeignKey(gt => gt.SkillLearningId),  // Явно указываем внешний ключ

                    j => j
                        .HasOne(g => g.Game)  // Настраиваем связь с Game
                        .WithMany()
                        .HasForeignKey(g => g.GameId),  // Явно указываем внешний ключ

                    j => j
                        .ToTable("GameSkillLearning")
                        .HasKey(t => new { t.GameId, t.SkillLearningId }) // Составной первичный ключ
                );

            modelBuilder.Entity<PendingGame>()
                .HasMany(g => g.SkillsLearning)
                .WithMany(gt => gt.PendingGames)
                .UsingEntity<PendingGameSkillLearning>(
                    j => j
                        .HasOne(gt => gt.SkillLearning)  // Настраиваем связь с GameType
                        .WithMany()
                        .HasForeignKey(gt => gt.SkillLearningId),  // Явно указываем внешний ключ

                    j => j
                        .HasOne(g => g.PendingGame)  // Настраиваем связь с Game
                        .WithMany()
                        .HasForeignKey(g => g.PendingGameId),  // Явно указываем внешний ключ

                    j => j
                        .ToTable("PendingGameSkillLearning")
                        .HasKey(t => new { t.PendingGameId, t.SkillLearningId }) // Составной первичный ключ
                );
        }
    }
}
