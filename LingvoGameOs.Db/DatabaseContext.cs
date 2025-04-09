using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Db
{
	public class DatabaseContext : DbContext
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
	}
}
