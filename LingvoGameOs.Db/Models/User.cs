using Microsoft.AspNetCore.Identity;

namespace LingvoGameOs.Db.Models
{
	public class User : IdentityUser
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string? Description { get; set; }
		public string? ImageURL { get; set; }
		public List<Game>? PlayerGames { get; set; }
        public List<UserGame> UserGames { get; set; }
        public List<Game>? DevGames { get; set; }
	}
}
