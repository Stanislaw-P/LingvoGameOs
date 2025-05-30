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
                .HasMany(g => g.Players)
                .WithMany(u => u.PlayerGames)
                .UsingEntity<PlayerGame>();

            modelBuilder.Entity<Game>()
                .HasMany(g => g.SkillsLearning)
                .WithMany(gt => gt.Games)
                .UsingEntity<GameSkillLearning>(
                    j => j
                        .HasOne(gt => gt.GameType)  // Настраиваем связь с GameType
                        .WithMany()
                        .HasForeignKey(gt => gt.GameTypeId),  // Явно указываем внешний ключ

                    j => j
                        .HasOne(g => g.Game)  // Настраиваем связь с Game
                        .WithMany()
                        .HasForeignKey(g => g.GameId),  // Явно указываем внешний ключ

                    j => j
                        .ToTable("GameSkillLearning")
                        .HasKey(t => new { t.GameId, t.GameTypeId }) // Составной первичный ключ
                );
        }
    }
}
