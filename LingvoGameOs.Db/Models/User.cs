using Microsoft.AspNetCore.Identity;

namespace LingvoGameOs.Db.Models
{
	public class User : IdentityUser
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string? Description { get; set; }
		public string? AvatarImgPath { get; set; }
		public List<Game>? PlayerGames { get; set; }
        public List<PlayerGame>? UserGames { get; set; }
        public List<Game>? DevGames { get; set; }
        public List<PendingGame>? DevPendingGames { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
