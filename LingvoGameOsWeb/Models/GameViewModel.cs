using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        [MaxLength(201)]
        public string Description { get; set; } = null!;
        public string Rules { get; set; } = null!;
        public User Author { get; set; } = null!;
        public DateTimeOffset PublicationDate { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public LanguageLevel? LanguageLevel { get; set; }
        public List<SkillLearning>? SkillsLearning { get; set; }
        public double AverageRaitingPlayers { get; set; }
        public string? CoverImagePath { get; set; }
        public List<string>? ImagesPaths { get; set; }
        public string? GameFilePath { get; set; }
        public Platform? GamePlatform { get; set; }
        public int NumberDownloads { get; set; }
        public List<User>? Players { get; set; }
        public string? GameFolderName { get; set; }
        public string? VideoUrl { get; set; }
        //public List<FavoriteGame>? FavoriteGames { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoritesCount{ get; set; }
        public bool IsActive { get; set; }
    }
}
