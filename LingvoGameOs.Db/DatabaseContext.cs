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
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Author)
                .WithMany(a => a.DevGames)
                .HasForeignKey(g => g.AuthorId);

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasKey(key => key.Id);
                entity.Property(key => key.Name).HasMaxLength(100).IsRequired();
                entity.Property(key => key.KeyPrefix).HasMaxLength(16).IsRequired();
                entity.Property(key => key.KeyHash).HasMaxLength(64).IsRequired();
                entity.Property(key => key.Scopes).HasMaxLength(256).IsRequired();
                entity.HasIndex(key => key.KeyHash).IsUnique();
                entity.HasIndex(key => key.UserId);
                entity.HasOne(key => key.User)
                    .WithMany(user => user.ApiKeys)
                    .HasForeignKey(key => key.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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
