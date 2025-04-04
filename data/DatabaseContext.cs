using LingvoGameOs.data.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.data
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Game> Games { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
			Database.Migrate();
		}
	}
}
