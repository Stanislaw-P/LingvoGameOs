using Microsoft.AspNetCore.Identity;

namespace LingvoGameOs.Db.Models
{
	public class User : IdentityUser
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string? Description { get; set; }
		public string? AvatarImgPath { get; set; }
        public List<GameHistory>? GamesHistory { get; set; }
        public List<Game>? DevGames { get; set; }
        public List<PendingGame>? DevPendingGames { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<FavoriteGame>? FavoriteGames { get; set; }
        public int TotalPoints { get; set; }
    }
}
