using LingvoGame.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGame.Db
{
    public class DatabaseContext : DbContext
    {
		public DbSet<Game> Games { get; set; } = null!;

		public DatabaseContext(DbContextOptions<DatabaseContext> optionsBuilder)
			:base(optionsBuilder)
		{
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=lingoGame.db");
		}
	}
}
