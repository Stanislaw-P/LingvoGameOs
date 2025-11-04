using System.Security.AccessControl;
using System;
using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Db.Models
{
	public class Game
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		[MaxLength(201)]
		public string Description { get; set; } = null!;
        public string Rules { get; set; } = null!;
        public User Author { get; set; }
        public string AuthorId { get; set; } = null!;
        public DateTimeOffset PublicationDate { get; set; }
		public DateTimeOffset LastUpdateDate { get; set; }
		public LanguageLevel? LanguageLevel { get; set; }
		public int LanguageLevelId { get; set; }
		public List<SkillLearning>? SkillsLearning { get; set; }
		public double AverageRaitingPlayers { get; set; }
        public string? CoverImagePath { get; set; }
        public List<string>? ImagesPaths { get; set; }
        public string? GameFilePath { get; set; }
		public string GameGitHubUrl { get; set; } = null!;
        public Platform? GamePlatform { get; set; }
		public int GamePlatformId { get; set; }
		public int NumberDownloads { get; set; }
		public List<GameHistory>? PlayersHistory { get; set; }
        public string? GameFolderName { get; set; }
        public string? VideoUrl { get; set; }
        public List<FavoriteGame>? FavoriteGames { get; set; }
        public int Port { get; set; }
		public bool IsActive { get; set; } = true;
        public int TotalRatingPoints { get; set; }
        public int TotalReviews { get; set; }
    }
}
