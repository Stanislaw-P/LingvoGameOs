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
        public LanguageLevel LanguageLevel { get; set; } = null!;
        public List<SkillLearning> SkillsLearning { get; set; } = null!;
        public double RaitingPlayers { get; set; }
        public double RaitingTeachers { get; set; }
        public string? CoverImagePath { get; set; }
        public List<string>? ImagesPaths { get; set; }
        public string? GameFilePath { get; set; }
        public Platform GamePlatform { get; set; } = null!;
        public int NumberDownloads { get; set; }
        public List<User>? Players { get; set; }
        public string? GameFolderName { get; set; }
        public string? VideoUrl { get; set; }
        //public List<FavoriteGame>? FavoriteGames { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoritesCount{ get; set; }
    }
}
